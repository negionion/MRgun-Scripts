using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Home : MonoBehaviour
{
    public Button fly;
    // Start is called before the first frame update
    void Start()
    {
        Screen.fullScreen = false;
        Screen.autorotateToPortrait = true;
        Screen.orientation = ScreenOrientation.Portrait;

        if(!BTsocket.isConnectedBLE(Constants.bleMicroBit))
        {
            fly.interactable = false;
        }
        else
        {
            fly.interactable = true;
        }  
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}

    }

    public void loadScene(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
    }
}
