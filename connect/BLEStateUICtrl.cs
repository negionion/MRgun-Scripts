using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BLEStateUICtrl : MonoBehaviour
{
    private RawImage state;
    public static void addBLEstateUI(GameObject _parent)
    {
        if(_parent.GetComponent<BLEStateUICtrl>() == null)
        {
            _parent.AddComponent<BLEStateUICtrl>();
        }       
    }
    // Start is called before the first frame update
    void Start()
    {
        GameObject uiPanel = new GameObject("stateUIpanel");
        uiPanel.transform.SetParent(BTsocket.getBTsocket(Constants.bleMicroBit).transform);
        uiPanel.AddComponent<RectTransform>();
        Canvas panel = uiPanel.AddComponent<Canvas>();
        panel.renderMode = RenderMode.ScreenSpaceOverlay;
        panel.sortingOrder = 15;
        uiPanel.AddComponent<CanvasScaler>();

        GameObject stateUI = new GameObject("stateUI");
        stateUI.transform.SetParent(uiPanel.transform);
        stateUI.AddComponent<RectTransform>();
        stateUI.AddComponent<CanvasRenderer>();
        state = stateUI.AddComponent<RawImage>();
        state.raycastTarget = false;
        
        InvokeRepeating("scanBLEstate", 1f, 1f);     
    }

    // Update is called once per frame
    private void scanBLEstate()
    {
        state.rectTransform.position = Vector3.zero;
        if(BTsocket.isConnectedBLE(Constants.bleMicroBit))
        {
            state.color = Color.green;
        }
        else
        {
            state.color = Color.red;
        }
    }
}
