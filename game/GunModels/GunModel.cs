using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using GoogleARCore;

public enum GunType
{
	AR = 0,
	RF,
	SG,
	EMPTY
}

public abstract class GunModel : MonoBehaviour
{
	//子彈數量、傷害、射程
	public int bullet, bulletMax, damage, shotDistance;
	//arCore使用
	protected Camera FirstPersonCamera;

	public AudioSource fireAudio;
	private Vector2 sightPos;

	//子物件多型func
	public abstract void fire(bool isFire);	
	public abstract void select();
	public abstract void reload();

	protected bool fireOK { get; set; }

	[SerializeField]
	protected float fireDelay;

	[SerializeField]
	protected PoolObj impactPool;

	//繼承緣故，子類別非必要不覆寫父類別的Unity基本Function，請改用ch開頭的方法，並要求子類別override
	void Start()
	{
		FirstPersonCamera = Camera.main;
		fireOK = true;
		chStart();
	}
	protected abstract void chStart();

	void Update()
	{
		chUpdate();
	}
	protected abstract void chUpdate();

	protected void fireCD()
	{
		StartCoroutine(fireTimer());
	}

	private IEnumerator fireTimer()    //開火冷卻
	{
		fireOK = false;
		yield return new WaitForSeconds(fireDelay);
		fireOK = true;
	}

	/*protected void trigAtkDamage(float _damage)
	{
		SingleObj<GunAtkDamage>.instance.evtAtkDamage(this, (int)_damage);
	}*/

	//計算遊戲中準星UI座標轉換到相機座標的位置
	protected Vector2 getSightPosToScreen()
	{
		return new Vector2(((GunControl.gunRay.position.x / Constants.uiWidth) + 0.5f) * Screen.width, ((GunControl.gunRay.position.y / Constants.uiHeight) + 0.5f) * Screen.height);
	}

	protected void gameSceneFire(Ray ray, float maxDistance, Action<RaycastHit> hitAct, Action noHitAct = null)
	{
		RaycastHit shotHit;
		if(Physics.Raycast(ray, out shotHit, maxDistance))
		{
			hitAct(shotHit);
		}
		else
		{
			noHitAct?.Invoke();
		}
	}

	protected void arCoreFire(Vector2 raycastPose, Action<TrackableHit> hitAct, Action noHitAct = null)
	{		
		TrackableHit shotHit;
		TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
			TrackableHitFlags.FeaturePointWithSurfaceNormal;
		if (Frame.Raycast(raycastPose.x, raycastPose.y, raycastFilter, out shotHit))
		{
			if ((shotHit.Trackable is DetectedPlane) &&
			Vector3.Dot(FirstPersonCamera.transform.position - shotHit.Pose.position,
				shotHit.Pose.rotation * Vector3.up) < 0)
			{
				Debug.Log("Hit at back of the current DetectedPlane");
			}
			else
			{
				if (shotHit.Trackable is DetectedPlane)
				{
					hitAct(shotHit);					
				}
			}
		}
		else
		{
			noHitAct?.Invoke();
		}

	}

}
