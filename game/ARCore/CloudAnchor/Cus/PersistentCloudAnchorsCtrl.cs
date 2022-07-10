using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using GoogleARCore.CrossPlatform;
using UnityEngine.Networking;


// 資料表：https://docs.google.com/spreadsheets/d/1ezdzfJgoh6mLK1__156iDnQSdUdz7A09ZdWpfCZbzgk/edit#gid=0
public class PersistentCloudAnchorsCtrl : MonoBehaviour
{
    private Dictionary<string, string> resolveSet;    //<cloud ID, gameobject Name>
    public string URL = "https://script.google.com/macros/s/AKfycbzB3s2X344DomkubvCuycm1iDmM0m2Z3foHGaNXYZeR-G8U5Mv0fUsR8mLE9aIqHxen3g/exec";
    public GameObject[] cloudAnchorPrefabs;

    public static string sessionID = "";
    public static bool resolveStartFlag = false;    //必須true才可啟動resolve功能
    public GameObject startPanel;   //地圖建構模式啟動的UIpanel

    private Dictionary<string, GameObject> cloudAnchorPrefabsMatch;
    private List<string> pendingTask;   //等待解析的anchor ID

    private void initiateCloudAnchorPrefabs()   //將gameobject傳換為字串id，以便後續直接透過string進行剖析或寫入
    {
        cloudAnchorPrefabsMatch = new Dictionary<string, GameObject>();
        foreach (var _prefab in cloudAnchorPrefabs)
        {
            cloudAnchorPrefabsMatch.Add(_prefab.name, _prefab);
        }
    }

    private IEnumerator loadResolveSet(string _sessionID, bool onNet)
    {
        bool isLoaded = false;
        resolveSet = new Dictionary<string, string>();
        if (onNet)   //從網路獲取
        {
            PCAsheetCmd("get", _sessionID,
            (response) =>
            {
                //序列化json
                response = "{\"target\":" + response + "}";
                List<PCAset> pcaSet = JsonUtility.FromJson<Serialization<PCAset>>(response).ToList();
                //將每一筆資料轉存到resolveSet中準備處理
                foreach (var pca in pcaSet)
                {
                    resolveSet.Add(pca.cloudID, pca.objName);
                    GameObject.Find("CAsDebugText").GetComponent<UnityEngine.UI.Text>().text += "\nPCAid = " + pca.cloudID;
                }

                isLoaded = true;
            });
        }
        else        //從本地獲取
        {
            //------test
            resolveSet.Add("123", "456");
            resolveSet.Add("abc", "def");
            //------test
            isLoaded = true;
        }
        while (!isLoaded) yield return 0;
    }

    [System.Serializable]
    private class PCAset
    {
        public string cloudID;
        public string objName;
    }


    public void PCAsheetCmd(string sel, string sessionID, System.Action<string> successAction = null, string cloudID = "", string objName = "")
    {
        string _url = string.Empty;
        switch (sel)
        {
            case ("get"):   //取得PCA資料集
                _url = URL + string.Format("?sel={0}&sessionID={1}",
                    sel, sessionID);
                break;
            case ("set"):   //存入PCA資料
                _url = URL + string.Format("?sel={0}&sessionID={1}&cloudID={2}&objName={3}",
                    sel, sessionID, cloudID, objName);
                break;
        }

        if (_url != string.Empty)
        {
            StartCoroutine(doGET(_url,
                (response) =>
                {
                    if (successAction != null)
                    {
                        successAction(response.text);
                    }
                }));
        }
    }

    private IEnumerator doGET(string _url, System.Action<DownloadHandler> responseAction = null)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(_url))
        {
            yield return webRequest.SendWebRequest();
            if (responseAction != null && webRequest.isDone)
            {
                responseAction(webRequest.downloadHandler);
            }
        }
    }

    public void doResolvingCloudAnchors()
    {
        StartCoroutine(ResolvingCloudAnchors());
    }

    public IEnumerator ResolvingCloudAnchors()    //啟動解析功能
    {
        if (sessionID == string.Empty)
            yield break;
        //初始化
        initiateCloudAnchorPrefabs();
        yield return StartCoroutine(loadResolveSet(sessionID, true));
        // No Cloud Anchor for resolving.
        if (resolveSet.Count == 0)
        {
            yield break;
        }

        // ARCore session is not ready for resolving.
        if (Session.Status != SessionStatus.Tracking)
        {
            yield break;
        }

        //先預載入要解析的地圖集，等待旗標才可進行解析(resolveStartFlag = true)
        while (!resolveStartFlag) yield return 0;

        Debug.LogWarning("resolve start");

        pendingTask = new List<string>();
        //註冊雲錨資料，並開始解析
        foreach (var _cloudanchor in resolveSet)
        {
            pendingTask.Add(_cloudanchor.Key);
            XPSession.ResolveCloudAnchor(_cloudanchor.Key).ThenAction(result =>
            {
                pendingTask.Remove(result.Anchor.CloudId);
                if (result.Response != CloudServiceResponse.Success)
                {
                    Debug.LogFormat("Faild to resolve cloud anchor {0} for {1}",
                        result.Anchor, result.Response);
                }
                else
                {
                    GameObject tmpObj;
                    Pose virtualPos = SingleObj<MRGun.CloudAnchor.ARCoreWorldOriginHelper>.obj.
                        _WorldToAnchorPose(new Pose(result.Anchor.transform.position, result.Anchor.transform.rotation));
                    if (cloudAnchorPrefabsMatch.TryGetValue(_cloudanchor.Value, out tmpObj))
                    {
                        GameObject _tempObj = null;
                        if (tmpObj.tag == Constants.tagEnemy)
                        {
                            try
                            {
                                _tempObj = SingleObj<EnemyGenerator>.obj.createEnemy(virtualPos.position);
                                if (_tempObj != null)
                                {
                                    _tempObj.transform.SetParent(null);
                                    //_enemy.transform.position += (Camera.main.transform.position - Frame.Pose.position);
                                    _tempObj.GetComponent<ProjectileCus>().closeAutoCalibrate();                                    
                                }
                                
                                MultiplayerCtrl.getLocalPlayer().GetComponent<MultiplayerPCACtrl>().CmdSyncPCA(result.Anchor.CloudId);
                            }
                            catch (System.Exception e)
                            {
                                GameObject.Find("CAsDebugText").GetComponent<UnityEngine.UI.Text>().text += "\n" + e.StackTrace;
                            }
                        }
                        else
                        {
                            _tempObj = Instantiate(tmpObj);
                            _tempObj.transform.SetParent(null);
                            _tempObj.transform.position = virtualPos.position;
                        }
                        
                        GameObject.Find("CAsDebugText").GetComponent<UnityEngine.UI.Text>().text += string.Format(
                            "\n{0} pos{1}", _tempObj.name, _tempObj.transform.position);
                        if(_tempObj)
                        {
                            //CreateAnchor(_tempObj.transform);
                        }
                    }
                }
            });
        }
        resolveSet.Clear();
    }

    public void cancelResolvePCA(string _cloudID)
    {
        XPSession.CancelCloudAnchorAsyncTask(_cloudID);
        pendingTask.Remove(_cloudID);
    }

    public void setSessionID(string _text = "")
    {
        sessionID = _text;
        if (startPanel)
            startPanel.SetActive(sessionID != string.Empty);
        Debug.Log("sessionID = " + sessionID);
    }

    public void CreateAnchor(Transform obj)
    {
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(obj.transform.position);

        // Raycasts against the location the object stopped.
        TrackableHit hit;
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
            TrackableHitFlags.FeaturePointWithSurfaceNormal;

        if (Frame.Raycast(screenPoint.x, screenPoint.y, raycastFilter, out hit))
        {
            // Uses hit pose and camera pose to check if hittest is from the
            // back of the plane, if it is, no need to create the anchor.
            if ((hit.Trackable is DetectedPlane) &&
                Vector3.Dot(Camera.main.transform.position - hit.Pose.position,
                    hit.Pose.rotation * Vector3.up) < 0)
            {
                Debug.Log("Hit at back of the current DetectedPlane");
            }
            else
            {
                // Creats an anchor to allow ARCore to track the hitpoint as understanding of
                // the physical world evolves.
                var anchor = hit.Trackable.CreateAnchor(hit.Pose);
                obj.transform.SetParent(anchor.transform, true);
            }
        }
    }
}


