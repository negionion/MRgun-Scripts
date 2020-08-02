using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BTManager : MonoBehaviour {
	public GameObject linkButton;					//按鈕預製
	private int linkButtonPos, linkBtnFragment; 	//按鈕出現位置y,每個按鈕的間距
	public RectTransform conBtnPanel;				//scroll view panel
	public Transform conBtnArch;					//按鈕錨點與父節點
	public Text BTlog;								//藍芽log

	public Text connectHint;					//連線好的addr
	private BTsocket btSoc;

	public Button disConnectedBtn;

	private static int cnt = 0, recvCnt = 0;
	// Use this for initialization
	void Start () {
		if(BTsocket.isConnectedBLE(Constants.bleMicroBit))
		{
			btSoc = BTsocket.getBTsocket(Constants.bleMicroBit);
			connectHint.text = btSoc.bleLinkData.address;
			disConnectedBtn.gameObject.SetActive(true);
		}
		else
		{
			linkButtonPos = -50;
			linkBtnFragment = 100;
			btSoc = BTsocket.getNewBTsocket(Constants.bleMicroBit, new BTprofile(Constants.bleMicroBit, true));
			Invoke("delayScan", 2f);
		}
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown(KeyCode.A))	//除錯用
		{
			addPeripheralButton("123","addr");
			conBtnPanel.sizeDelta = new Vector2(0, conBtnPanel.sizeDelta.y + linkBtnFragment);
		}

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			SceneManager.LoadSceneAsync("home");
		}
		
		BTlog.text = btSoc.BTLog;
		
	}

	public void testMethod()
	{
		btSoc.getReceiveText((data) => {recvCnt = int.Parse(data);});
		btSoc.writeCharacteristic("#");
		cnt++;		
		connectHint.text = "傳送資料次數：" + cnt.ToString() + " 回傳次數：" + recvCnt.ToString();
	}

	public void addPeripheralButton(string addr, string name)
	{
		GameObject newPeripheral = Instantiate(linkButton);
		newPeripheral.transform.SetParent(conBtnArch);
		newPeripheral.transform.localScale = new Vector3(1, 1, 1);
		newPeripheral.transform.localPosition = new Vector2(0, linkButtonPos);
		newPeripheral.GetComponent<LinkButton>().address = addr;
		newPeripheral.GetComponent<LinkButton>().name = name;
		newPeripheral.GetComponentInChildren<Text>().text = name + "\n" + addr.ToUpper();

		linkButtonPos -= linkBtnFragment;
	}

	private void delayScan()
	{
		btSoc.scan((addr, name) =>
		{
			addPeripheralButton(addr, name);
			conBtnPanel.sizeDelta = new Vector2(0, conBtnPanel.sizeDelta.y + linkBtnFragment);
			/*if(addr.Equals(PlayerPrefs.GetString("preConnectMAC")))
			{
				preConnectBtn.SetActive(true);
			}*/
		});
	}

	public void connectAct(LinkButton data, System.Action connectedAct = null)
	{		
		btSoc.connect(data.address);

		//-----測試功能-----
		connectHint.text = data.address;

		if(connectedAct != null)
			StartCoroutine(waitLoop(connectedAct));
	}

	public IEnumerator waitLoop(System.Action connectedAct)
	{
		while(!BTsocket.isConnectedBLE(Constants.bleMicroBit))
		{
			yield return 0;
		}
		btSoc.subscribe();
		disConnectedBtn.gameObject.SetActive(true);
		connectedAct();
		//----------------------------
		/*cnt = 0;
		recvCnt = 0;
		InvokeRepeating("testMethod", 2f, 1f);*/
	}

	public void disConnected()
	{
		btSoc.disConnect();
		SceneManager.LoadSceneAsync("connect");
		//Invoke("delayLoad", 2f);

		//------------------
		/*CancelInvoke("testMethod");
		connectHint.text = "累積傳送次數：" + cnt.ToString() + " 回傳次數：" + recvCnt.ToString();*/
	}

	public void preConnect()
	{
		btSoc.connect(PlayerPrefs.GetString("preConnectMAC"));
	}
}
