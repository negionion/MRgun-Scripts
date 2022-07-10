using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Process flow (host)
/// resolveRegiste -> multiPlayer -> resetPos -> setWorldOrigin -> resolveStart
/// (UI--------------------------)  (Gun--------------------------------------)
/// 
/// Process flow (client)
/// resolveRegiste -> multiPlayer -> resetPos -> resolveWorldOrigin -> resolveStart
/// (UI--------------------------)  (Gun------------------------------------------)
/// </summary>
public class MultiPlayerUICtrl : MonoBehaviour
{
    public bool debugModeFlag = false;
    public Canvas resolvePanel, gamePanel, multiplayerPanel;
    // Start is called before the first frame update
    void Start()
    {
        switchResolvePanel();
    }

    public void debugMode()
    {
        if(debugModeFlag)
        {
            SingleObj<ARCoreCtrl>.obj.resetPose();
            SingleObj<ARCoreCtrl>.obj.resetPose();
            SingleObj<MRGun.CloudAnchor.CloudAnchorsExampleController>.obj.SetWorldOrigin(gameObject.transform);
            switchGamePanel();
        }
    }  

    public void switchResolvePanel()
    {        
        resolvePanel.enabled = true;
        gamePanel.enabled = false;
        multiplayerPanel.enabled = false;
        SingleObj<PersistentCloudAnchorsCtrl>.obj.setSessionID();
    }

    public void switchMultiplayerPanel()
    {
        resolvePanel.enabled = false;
        gamePanel.enabled = false;
        multiplayerPanel.enabled = true;
    }

    public void switchGamePanel()
    {
        resolvePanel.enabled = false;
        gamePanel.enabled = true;
        multiplayerPanel.enabled = false;
    }
}
