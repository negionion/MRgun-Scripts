using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;
public class Home : MonoBehaviour
{
    public Button gameBtn;
    public Button gameModeBtn;
    [SerializeField]
    private bool debugMode = false;
    // Start is called before the first frame update
    void Start()
    {
        if (XRSettings.loadedDeviceName != XRSettings.supportedDevices[1])
			XRSettings.LoadDeviceByName(XRSettings.supportedDevices[1]);
		if (XRSettings.loadedDeviceName == XRSettings.supportedDevices[1])
			XRSettings.enabled = false;
        Screen.fullScreen = false;
        Screen.autorotateToPortrait = true;
        Screen.orientation = ScreenOrientation.Portrait;
		
        if(!debugMode)
            gameBtn.interactable = BTsocket.isConnectedBLE(Constants.bleMicroBit);
 
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
        
        gameModeBtn.GetComponentInChildren<Text>().text = GameUI.XRisEnabled ? "VR" : "Screen";
    }

    public void XRswitch()
    {
        GameUI.XRisEnabled = !GameUI.XRisEnabled;       
    }

    public void loadScene(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
    }
}
