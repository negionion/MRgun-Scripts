using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    public static bool XRisEnabled = false;
    // Start is called before the first frame update
    void Start()
    {
        Screen.fullScreen = true;
        Screen.autorotateToPortrait = false;
        Screen.orientation = ScreenOrientation.Landscape;
        Screen.orientation = ScreenOrientation.LandscapeLeft;

        UnityEngine.XR.XRSettings.enabled = XRisEnabled;
        UnityEngine.XR.XRDevice.DisableAutoXRCameraTracking(Camera.main, true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
