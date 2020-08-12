using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunControl : MonoBehaviour
{
	//槍枝對應準星位置和槍枝轉動控制
	public static MotionData mtData;
	private BTsocket ble;
	public GameObject gun;
	private Vector3 gunRotateOld = Vector3.zero;
	public float rotateDeviation = 0.1f;
	public static Pose gunRay;
	public GameObject sight;
	public GameObject gunSetPos;
	public Camera mainCamera;   //FPS camera
	
	//槍枝狀態控制
	public event System.EventHandler<char> gunStateChangedEvt;
	private char oldState = '0';

	//演算法參數
	private int stAngle = -1;        //gun定位中心點(零點), camera y軸角度
	private float range = 4f, lerpSpeed = 15f, cameraAngle, tempGunRotate;	//武器可動範圍, 滑順補正倍率, 相機定位角度, 武器前次監測角度

	void Awake()
	{
		Screen.fullScreen = true;
		Screen.autorotateToPortrait = false;
		Screen.orientation = ScreenOrientation.Landscape;
		Screen.orientation = ScreenOrientation.AutoRotation;
	}

	void Start()
    {
		gunRay.position = Vector3.zero;

		ble = BTsocket.getBTsocket(Constants.bleMicroBit);
		cameraAngle = mainCamera.transform.eulerAngles.y;
	}



    // Update is called once per frame
    void Update()
    {
		gunSetPos.transform.position = mainCamera.transform.position;   //讓槍跟著鏡頭移動

		gunSetPos.transform.eulerAngles = new Vector3(      //調整槍枝讓其對準
			gunSetPos.transform.eulerAngles.x,
			mainCamera.transform.eulerAngles.y,
			gunSetPos.transform.eulerAngles.z);

		//---------使用鍵盤模擬輸入，測試用-----------------!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

		if (Input.inputString.Length > 0 && oldState != Input.inputString.ToCharArray()[0])
		{
			gunStateChangedEvt?.Invoke(this, Input.inputString.ToCharArray()[0]); //觸發事件，送出槍目前狀態
			oldState = Input.inputString.ToCharArray()[0];
			Debug.Log(Input.inputString);
		}

		//-------------------------------------------------!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

		if (!BTsocket.isConnectedBLE(Constants.bleMicroBit))
			return;		

		ble.getReceiveText((data) =>
		{
			gunMotionCtrl(data);			
		});

		float deltaAngle = Mathf.Abs(mainCamera.transform.eulerAngles.y - cameraAngle);

		if (!(deltaAngle < 10 || deltaAngle > 350) && Mathf.Abs(mtData.angle) > 10)
		{
			cameraAngle = mainCamera.transform.eulerAngles.y;   //reset
			set_stAngle();  //reset
		}

		

	}

	private Queue<float> rotateXAvg = new Queue<float>(), rotateYAvg = new Queue<float>();
	private float rotateAvg(Queue<float> avgQueue, float _rotate)
	{
		float ans = 0;
		avgQueue.Enqueue(_rotate);
		if(avgQueue.Count > 5)	//平均的佇列數量
		{
			avgQueue.Dequeue();
		}
		foreach(float val in avgQueue)
		{
			ans += val;
		}
		
		ans /= avgQueue.Count;
		return ans;
	}

	public void gunMotionCtrl(string data)
	{
		mtData = getMotionData(data);	//將動作資料反序列化為MotionData物件
		float xRange = 40f;				//x軸向可動範圍
		float yRange = 35f;				//y軸向可動範圍
		//加速度x軸參數正規化，並提高1.5倍靈敏度，調整區間限制於±xRange中
		float rotateX = rotateAvg(rotateXAvg, Mathf.Clamp(-(mtData.x / 1024f) * xRange * 1.5f, -xRange, xRange));
		/*if((rotateX - gunRotateOld.x) / gunRotateOld.x < rotateDeviation)	//誤差過小則保持前次狀態(防止靜止時晃動)
			rotateX = gunRotateOld.x;*/
		//計算遊戲中武器的左右轉動角度，正規化於±30之間，提高range倍靈敏度，限制於±30度
		float rotateY = rotateAvg(rotateYAvg, Mathf.Clamp((mtData.angle / 180f) * yRange * range, -yRange, yRange));		
		/*if((rotateY - gunRotateOld.y) / gunRotateOld.y < rotateDeviation / 2f)	//誤差過小則保持前次狀態(防止靜止時晃動)
			rotateY = gunRotateOld.y;*/
		//將算完的結果存起來，用作判斷下次誤差
		//gunRotateOld.Set(rotateX, rotateY, 0);

		//使武器轉動滑順
		gun.transform.localEulerAngles = new Vector3(
			Mathf.LerpAngle(gun.transform.localEulerAngles.x, rotateX, Time.deltaTime * lerpSpeed),
			Mathf.LerpAngle(gun.transform.localEulerAngles.y, rotateY, Time.deltaTime * lerpSpeed),
			0);
		//準星位置對應，準星可動範圍定為1600*800(Canva為1920*1080)
		gunRay.position = new Vector3(
			Mathf.Lerp(gunRay.position.x, Mathf.Clamp((mtData.angle / 180f) * 800f * range, -800, 800), Time.deltaTime * lerpSpeed),
			Mathf.Lerp(gunRay.position.y, -(rotateX / xRange) * 400, Time.deltaTime * lerpSpeed),
			0);
		sight.GetComponent<RectTransform>().localPosition = gunRay.position;

		if (oldState != mtData.gunSt)
		{
			gunStateChangedEvt(this, mtData.gunSt); //觸發事件，送出槍目前狀態
			oldState = mtData.gunSt;
		}

	}

	public void gunMotionCtrl(MotionData mtData)
	{
		float xRange = 40f;             //x軸向可動範圍
		float yRange = 35f;				//y軸向可動範圍
		//加速度x軸參數正規化，並提高1.5倍靈敏度，調整區間限制於±xRange中
		float rotateX = rotateAvg(rotateXAvg, Mathf.Clamp(-(mtData.x / 1024f) * xRange * 1.5f, -xRange, xRange));
		/*if((rotateX - gunRotateOld.x) / gunRotateOld.x < rotateDeviation)	//誤差過小則不做動(防止靜止時晃動)
			rotateX = gunRotateOld.x;*/
		//計算遊戲中武器的左右轉動角度，正規化於yRange之間，提高range倍靈敏度，限制於±30度
		float rotateY = rotateAvg(rotateYAvg, Mathf.Clamp((mtData.angle / 180f) * yRange * range, -yRange, yRange));		
		/*if((rotateY - gunRotateOld.y) / gunRotateOld.y < rotateDeviation / 2f)	//誤差過小則不做動(防止靜止時晃動)
			rotateY = gunRotateOld.y;*/
		//將算完的結果存起來，用做判斷下次誤差
		//gunRotateOld.Set(rotateX, rotateY, 0);
		
		//使武器轉動滑順
		gun.transform.localEulerAngles = new Vector3(
			Mathf.LerpAngle(gun.transform.localEulerAngles.x, rotateX, Time.deltaTime * lerpSpeed),
			Mathf.LerpAngle(gun.transform.localEulerAngles.y, rotateY, Time.deltaTime * lerpSpeed),
			0);
		//準星位置對應，準星可動範圍定為800*400(Canva為1920*1080)
		gunRay.position = new Vector3(
			Mathf.Lerp(gunRay.position.x, Mathf.Clamp((mtData.angle / 180f) * 800f * range, -800, 800), Time.deltaTime * lerpSpeed),
			Mathf.Lerp(gunRay.position.y, -(rotateX / xRange) * 400, Time.deltaTime * lerpSpeed),
			0);
		sight.GetComponent<RectTransform>().localPosition = gunRay.position;

		if (oldState != mtData.gunSt)
		{
			gunStateChangedEvt(this, mtData.gunSt); //觸發事件，送出槍目前狀態
			oldState = mtData.gunSt;
		}

	}





	public void set_stAngle(int data = -1)
	{
		stAngle = data;
	}

	public class MotionData
	{
		public int x, y;		//-1024~1024, -1024~1024
		public char gunSt;      //(0:null, @:板機, P:霰彈, S:步槍, A:自動步槍, R:換彈)
		public float angle;		//0~360

		public MotionData(int _x, int _y, float _a, char _gunSt)
		{
			x = _x;
			y = _y;
			angle = _a;
			gunSt = _gunSt;
		}
	}

	public MotionData getMotionData(string data)
	{
		string[] mtData = data.Split(',');

		if (stAngle < 0)
		{
			set_stAngle(int.Parse(mtData[2]));
		}
		float maxAngle = (stAngle + 180) % 360;

		float rotateAngle = int.Parse(mtData[2]);
		if (stAngle < 180)
		{
			if (rotateAngle > stAngle && rotateAngle < maxAngle)    //R
			{
				rotateAngle = rotateAngle - stAngle;
			}
			else                                    //L
			{
				if (rotateAngle >= maxAngle)
				{
					rotateAngle = (rotateAngle - 360) - stAngle;
				}
				else
				{
					rotateAngle = rotateAngle - stAngle;
				}
			}
		}
		else
		{
			if (rotateAngle > maxAngle && rotateAngle < stAngle)    //L
			{
				rotateAngle = rotateAngle - stAngle;
			}
			else                                        //R
			{
				if (rotateAngle <= maxAngle)
				{
					rotateAngle = (360 - stAngle) + rotateAngle;
				}
				else
				{
					rotateAngle = rotateAngle - stAngle;
				}
			}
		}

		return new MotionData(int.Parse(mtData[0]), int.Parse(mtData[1]), rotateAngle, mtData[3].ToCharArray()[0]);
	}

}
