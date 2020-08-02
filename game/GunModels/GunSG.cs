using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSG : GunModel
{
	public Transform muzzleFlash;
	public ParticleSystem spark;
	public Animator sightAnime;
	public readonly string animeCtrlName = "fireSG";
	private bool loaded = false;

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
			bullet--;
			fireCD();

			StartCoroutine(fireAction());
		}
		
	}

	private IEnumerator fireAction()
	{
		Vector2 raycastPose = new Vector2(GunControl.gunRay.position.x + (Screen.width / 2), GunControl.gunRay.position.y + (Screen.height / 2));
		GameObject hitEnemy = null;
		Ray ray = FirstPersonCamera.ScreenPointToRay(new Vector3(raycastPose.x, raycastPose.y, 0));
		gameSceneFire(ray, shotDistance, (hit) =>
		{
			spark.transform.position = hit.point;
			spark.transform.LookAt(hit.point - hit.normal);
			spark.transform.Translate(Vector3.back * 0.01f);
			spark.Play();
			hitEnemy = hit.collider.gameObject;
		});

		if (hitEnemy == null)
		{
			arCoreFire(raycastPose, (hit) =>
			{
				spark.transform.position = hit.Pose.position;
				spark.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.Pose.position);
				spark.Play();
			});
		}
		if (loaded)
		{
			hitEnemy?.GetComponent<EnemyRecvDamage>().recvDamage(damage * 1.5f);
		}
		else
		{
			hitEnemy?.GetComponent<EnemyRecvDamage>().recvDamage(damage);
		}
		loaded = false;

		sightAnime.SetBool(animeCtrlName, true);
		muzzleFlash.gameObject.SetActive(true);

		yield return new WaitForSeconds(0.3f);

		spark.Stop();
		sightAnime.SetBool(animeCtrlName, false);
		muzzleFlash.gameObject.SetActive(false);
	}

	public override void reload()
	{
		bullet = bulletMax;
		Debug.Log("Reload");
	}

	public override void select()
	{
		if (bullet > 0)
			loaded = true;
		Debug.Log("Select SG");
	}
}
