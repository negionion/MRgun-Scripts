using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAR : GunModel
{
	public Transform muzzleFlash;
	public ParticleSystem spark;
	public Animator sightAnime;
	public readonly string animeCtrlName = "fireAR";

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
			spark.Stop();
			sightAnime.SetBool(animeCtrlName, false);
			muzzleFlash.gameObject.SetActive(false);			
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
		Debug.Log("Select AR");
	}

	private IEnumerator fireAction()
	{
		float timing = 0;
		Vector2 raycastPose = new Vector2(GunControl.gunRay.position.x + (Screen.width / 2), GunControl.gunRay.position.y + (Screen.height / 2));
		GameObject hitEnemy = null;
		muzzleFlash.gameObject.SetActive(true);
		sightAnime.SetBool(animeCtrlName, true);
		Ray ray = FirstPersonCamera.ScreenPointToRay(new Vector3(raycastPose.x, raycastPose.y, 0));
		while (bullet > 0)
		{
			raycastPose = new Vector2(GunControl.gunRay.position.x + (Screen.width / 2), GunControl.gunRay.position.y + (Screen.height / 2));
			ray = FirstPersonCamera.ScreenPointToRay(new Vector3(raycastPose.x, raycastPose.y, 0));
			muzzleFlash.Rotate(0, 0, 20);
			spark.Stop();
			gameSceneFire(ray, shotDistance, (hit) =>
			{				
				spark.transform.position = hit.point;
				spark.transform.LookAt(hit.point - hit.normal);
				spark.transform.Translate(Vector3.back * 0.01f);
				spark.Play();
				hitEnemy = hit.collider.gameObject;
			});

			if(hitEnemy == null)
			{
				arCoreFire(raycastPose, (hit) =>
				{
					spark.transform.position = hit.Pose.position;
					spark.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.Pose.position);
					spark.Play();
				});
			}

			if (timing >= fireDelay)
			{
				timing = 0;
				bullet--;
				//對怪物造成傷害
				hitEnemy?.GetComponent<EnemyRecvDamage>().recvDamage(damage);
			}
			hitEnemy = null;
			timing += Time.deltaTime;
			yield return 0;
		}
		spark.Stop();
		sightAnime.SetBool(animeCtrlName, false);
		muzzleFlash.gameObject.SetActive(false);
	}
}
