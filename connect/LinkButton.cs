using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LinkButton : MonoBehaviour {

	public string address;
	public string BLEname;
	// Use this for initialization
	void Start()
	{
		Button btn = GetComponent<Button>();
		btn.onClick.AddListener(OnClick);
	}

	// Update is called once per frame
	void Update () {
		
	}

	private void OnClick()
	{
		//GameObject.Find(Constants.bleMicroBit).GetComponent<BTsocket>().connect(address);		
		//-----測試功能-----
		if(!BTsocket.isConnectedBLE(Constants.bleMicroBit))
		{
			GameObject.Find("EventSystem").GetComponent<BTManager>().connectAct(this, 
			() =>
			{
				this.GetComponentInChildren<Text>().color = Color.blue;
			});
		}
		else
		{
			GameObject.Find("EventSystem").GetComponent<BTManager>().disConnected();
			this.GetComponentInChildren<Text>().color = Color.black;
		}
		//this.GetComponentInChildren<Text>().color = Color.blue;

		//PlayerPrefs.SetString("preConnectMAC", address);

	}

}
