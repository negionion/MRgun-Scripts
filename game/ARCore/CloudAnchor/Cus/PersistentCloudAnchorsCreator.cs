using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PersistentCloudAnchorsCreator : MonoBehaviour
{
    private PersistentCloudAnchorsCtrl PCActrl;
    private GameObject setPrefab;

    public Text selText;
    public GameObject selPanel;
    public Button switchBtn;


    // Start is called before the first frame update
    void Start()
    {
        PCActrl = SingleObj<PersistentCloudAnchorsCtrl>.obj;
    }


    public bool addPCA(Transform _transform)
    {
        if (setPrefab == null)
            return false;
        GameObject gobj = Instantiate(setPrefab, _transform); //建立錨點
        foreach (var _component in gobj.GetComponents<Component>())
        {
            if (_component.GetType() == typeof(Transform))
            {

            }
            else
            {
                Destroy(_component);
            }
        }
        return true;

    }

    public void sharePCA(string _cloudID, Action<string> responseAct = null)
    {
        SingleObj<PersistentCloudAnchorsCtrl>.obj.
                PCAsheetCmd("set", PersistentCloudAnchorsCtrl.sessionID,
                (response) =>
                {
                    if (response == "success")
                    {
                        responseAct(response);
                    }
                },
                _cloudID, setPrefab.name);
    }

    public void selSetPrefab(GameObject _prefab)
    {
        setPrefab = _prefab;
        selText.text = string.Format("目前選擇物件：\n{0}", setPrefab.name);
    }

    public void switchSelPanel()
    {
        selPanel.SetActive(!selPanel.activeSelf);
        if (selPanel.activeSelf)
        {
            switchBtn.GetComponentInChildren<Text>().text = "Close";
        }
        else
        {
            switchBtn.GetComponentInChildren<Text>().text = "Items";
        }
    }
}
