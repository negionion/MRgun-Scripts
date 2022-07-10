using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using GoogleARCore.Examples.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Networking;

public class EnemyGenerator : MonoBehaviour
{
    [SerializeField]
    private Camera FirstPersonCamera;
    public static bool generateEnemyFlag { private set; get; } = false;   //是否生成怪物

    public float distance = 2f;
    public float interval = 5f; //生成怪物間隔時間
    public int enemyMaxCount = 5;
    public float scanScale = 1f;    //Enemy size
    public GameObject enemyPrefab;
    [SerializeField]
    private Bounds enemyBounds;

    public static List<Enemy> enemys = new List<Enemy>();
    public Text debugText;
    public Text scanText;

    // Start is called before the first frame update
    void Start()
    {
        FirstPersonCamera = Camera.main;
        generateEnemyFlag = true;
        //enemyBounds = enemyCollider.bounds;
        float maxExtents = Mathf.Max(enemyBounds.extents.x, enemyBounds.extents.z);
        if (scanScale <= maxExtents)
        {
            scanScale = maxExtents * 2;
        }
        StartCoroutine(generateHandler());

    }

    private IEnumerator generateHandler()
    {
        //Multiplayer
        if(Game.mode != Mode.PVP)
        {
            while(!MultiplayerCtrl.isConnected)     //waiting connect
            {
                yield return new WaitForSeconds(1f);
            }
        }
        else
        {
            yield break;
        }
        //-------------------------

        RaycastHit hit;
        bool generatedFlag = false;             
        yield return new WaitForSeconds(20f);    //Delay
        while (generateEnemyFlag)
        {
            //刷新AR的虛擬環境碰撞體(深度感測)
            SingleObj<DepthMeshColliderCus>.obj.ScanDepthCollider();
            yield return new WaitForSeconds(0.1f);

            generatedFlag = false;
            Ray ray = FirstPersonCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            if (Physics.Raycast(ray, out hit, 100))
            {
                float hitDistance = Vector3.Distance(FirstPersonCamera.transform.position, hit.point);

                //sim
                //hitDistance = distance;
                //hit.point = FirstPersonCamera.transform.position + new Vector3(0, 0, 2); 
                //----------------

                //Debug.Log("距離" + hitDistance);
                if (hitDistance >= distance && hitDistance <= distance * 10)
                {
                    Vector3 pos = scanAround(hit.point);
                    if (pos != Vector3.zero)
                    {
                        createEnemy(pos);
                        generatedFlag = true;
                    }
                }
                else if (hitDistance < distance && hitDistance >= 0.5f)
                {
                    Vector3 pos = scanAround(hit.point + (Vector3.up * enemyBounds.size.y) + (Vector3.forward * distance));
                    if (pos != Vector3.zero)
                    {
                        createEnemy(pos);
                        generatedFlag = true;
                    }
                }
            }
            if (generatedFlag)      //判斷經過interval時間後，是否已有生成敵人，否則頻率改為每秒偵測
                yield return new WaitForSeconds(interval);
            else
                yield return new WaitForSeconds(1f);
        }
    }

    private Vector3 scanAround(Vector3 center)
    {
        Vector3 ans = Vector3.zero, tmp = center;
        RaycastHit hit;
        Collider[] scanOverlap = new Collider[3];
        Vector3[] scanPosDelta = new Vector3[8];
        scanPosDelta[0].Set(0, 0, scanScale);
        scanPosDelta[1].Set(0, 0, -scanScale);
        scanPosDelta[2].Set(scanScale, 0, 0);
        scanPosDelta[3].Set(-scanScale, 0, 0);
        scanPosDelta[4].Set(scanScale, 0, scanScale);
        scanPosDelta[5].Set(scanScale, 0, -scanScale);
        scanPosDelta[6].Set(-scanScale, 0, scanScale);
        scanPosDelta[7].Set(-scanScale, 0, -scanScale);

        Vector3 scanCenterDelta = new Vector3(0, enemyBounds.size.y / 2, 0); //掃描下方有無地板的碰撞體中心
                                                                             //---------------------------
                                                                             //scanText.text = "collide " + FirstPersonCamera.transform.position + "\n";

        //---------------------------

        for (int i = 0; i < scanPosDelta.Length; i++)
        {
            GeneralFunc.Swap<Vector3>(ref scanPosDelta[i], ref scanPosDelta[UnityEngine.Random.Range(0, scanPosDelta.Length)]);
        }

        foreach (Vector3 delta in scanPosDelta)
        {
            tmp = center + delta;

            //scanText.text += tmp + " : ";
            if (Physics.Raycast(tmp, FirstPersonCamera.transform.position - tmp, out hit, 100))
            {
                //scanText.text += hit.transform.tag + "\n";  //test
                if (hit.transform.tag == Constants.tagPlayer)
                {
                    //scanText.text += Physics.OverlapSphereNonAlloc(tmp, 0.3f, scanOverlap) + "\n";  //test
                    //檢測座標周圍是否直接卡到碰撞體內，提高掃描中心至敵人模型高度的25%位置，掃描半身範圍的圓球內有無碰撞
                    if (Physics.OverlapSphereNonAlloc(tmp + (scanCenterDelta / 2), (enemyBounds.size.y / 4), scanOverlap) == 0)
                    {
                        //檢測座標正下方有無地板，有地板才生成
                        if (Physics.Raycast(tmp, Vector3.down, out hit, enemyBounds.extents.y))
                        {
                            RaycastHit[] hits = new RaycastHit[4];
                            Physics.Raycast(tmp + new Vector3(enemyBounds.extents.x / 2f, 0, enemyBounds.extents.z / 2f), Vector3.down, out hits[0], enemyBounds.extents.y);
                            Physics.Raycast(tmp + new Vector3(-enemyBounds.extents.x / 2f, 0, enemyBounds.extents.z / 2f), Vector3.down, out hits[1], enemyBounds.extents.y);
                            Physics.Raycast(tmp + new Vector3(enemyBounds.extents.x / 2f, 0, -enemyBounds.extents.z / 2f), Vector3.down, out hits[2], enemyBounds.extents.y);
                            Physics.Raycast(tmp + new Vector3(-enemyBounds.extents.x / 2f, 0, -enemyBounds.extents.z / 2f), Vector3.down, out hits[3], enemyBounds.extents.y);
                            int sucessPoint = 0;
                            float avgDistance = 0, minDistance = 0;
                            for (int scanCnt = 0; scanCnt < hits.Length; scanCnt++)
                            {
                                if (hits[scanCnt].transform?.tag == Constants.tagARCollider)
                                {
                                    sucessPoint++;
                                    float distance = Mathf.Abs(tmp.y - hits[scanCnt].point.y);  //只算垂直距離
                                    //Debug.Log(distance);
                                    avgDistance += distance;
                                    if (distance < minDistance)
                                        minDistance = distance;

                                }
                            }
                            avgDistance /= sucessPoint; //計算平均距離

                            if (sucessPoint >= hits.Length - 1 && (avgDistance - minDistance) <= 0.4f)   //必須通過3個點，且平均值-最小值誤差<0.4才可生成(無大的高低落差)
                            {

                                ans = tmp;
                                return ans; //直接中斷送出目標位置點
                            }
                        }
                    }
                }
            }
        }

        return ans;
    }

    public GameObject createEnemy(Vector3 pos)
    {
        GameObject enemy = null;
        if (FindObjectsOfType<Enemy>().Length < enemyMaxCount)
        {
            enemy = Instantiate(enemyPrefab, pos, Quaternion.identity);
        }
        else
        {
            var delEnemy = enemys[0];
            enemys.RemoveAt(0);
            Destroy(delEnemy.gameObject);  //移除最早出現的敵人       
            enemy = Instantiate(enemyPrefab, pos, Quaternion.identity);
        }

        enemy.GetComponent<EnemyMultiplayerCtrl>().targetId = MultiplayerCtrl.getLocalNetId();
        return enemy;
    }
}
