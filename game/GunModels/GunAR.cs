using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAR : GunModel
{	
	public ParticleSystem muzzleFire;
	public ParticleSystem impact;
	public Transform impactPos;
	public Animator sightAnime;
	public readonly string animeCtrlName = "fireAR";

	public UnityEngine.UI.Text colliderLog;

	protected override void chStart()
	{

	}
	protected override void chUpdate()
	{
		
	}

	public override void fire(bool isFire)
	{
		if (isFire && fireOK && bullet > 0)
		{			
			StartCoroutine("fireAction");
			fireCD();
		}
		else
		{
			StopCoroutine("fireAction");
			fireAudio.Stop();
			sightAnime.SetBool(animeCtrlName, false);
			muzzleFire.gameObject.SetActive(false);
			impact.gameObject.SetActive(false);			
		}
		Debug.Log("Fire = " + isFire.ToString());
	}

	public override void reload()
	{
		bullet = bulletMax;
		Debug.Log("Reload");
	}

	public override void select()
	{
		sightAnime.SetBool(animeCtrlName, false);
		Debug.Log("Select AR");
		fireOK = true;
	}

	private IEnumerator fireAction()
	{
		// 更新AR環境碰撞體，等待3幀
		SingleObj<DepthMeshColliderCus>.instance.ScanDepthCollider();
		yield return new WaitForSeconds(0.1f);

		float timing = 0;
		Vector2 raycastPose = getSightPosToScreen();
		GameObject hitEnemy = null;
		muzzleFire.gameObject.SetActive(true);
		muzzleFire.Play();
		impact.gameObject.SetActive(true);
		fireAudio.Play();
		sightAnime.SetBool(animeCtrlName, true);
		Ray ray = FirstPersonCamera.ScreenPointToRay(new Vector3(raycastPose.x, raycastPose.y, 0));
		while (bullet > 0)
		{
			
			raycastPose = getSightPosToScreen();
			Debug.Log(raycastPose);
			ray = FirstPersonCamera.ScreenPointToRay(new Vector3(raycastPose.x, raycastPose.y, 0));
			gameSceneFire(ray, shotDistance, 
			(hit) =>
			{				
				impactPos.position = hit.point;
				impactPos.LookAt(hit.point - hit.normal);
				impactPos.transform.Translate(Vector3.back * 0.01f);
				if(!impact.isPlaying)
				{
					impact.Play();
				}
				hitEnemy = hit.collider.gameObject;
				colliderLog.text = hit.point.ToString();
			}, 
			() => {impact.Stop(); });
			
			/*if(hitEnemy == null)	//舊版作法，使用AR平面感測
			{
				arCoreFire(raycastPose, (hit) =>
				{
					spark.transform.position = hit.Pose.position;
					spark.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.Pose.position);
					spark.Play();
				});
			}*/

			if (timing >= fireDelay)
			{
				timing = 0;
				bullet--;
				//刷新AR的虛擬環境碰撞體(深度感測)
				SingleObj<DepthMeshColliderCus>.instance.ScanDepthCollider();
				//對怪物造成傷害
				hitEnemy?.GetComponent<Enemy>()?.recvDamage(damage);
				hitEnemy?.GetComponent<BoomBox>()?.recvDamage(damage);
				//彈孔殘留效果，延遲5秒後消失(請參考ImpactShowDelay.cs)
				if(hitEnemy.tag == Constants.tagARCollider)
				{
					GameObject impactDelay = impactPool.getObj();
					impactDelay.transform.position = impactPos.position;
					impactDelay.transform.rotation = impactPos.rotation;
				}
			}
			hitEnemy = null;
			timing += Time.deltaTime;
			yield return 0;
		}
		fireAudio.Stop();
		muzzleFire.Stop();
		impact.Stop();
		muzzleFire.gameObject.SetActive(false);
		impact.gameObject.SetActive(false);
		sightAnime.SetBool(animeCtrlName, false);

	}
}
