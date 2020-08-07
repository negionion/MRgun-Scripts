using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunRF : GunModel
{
	public ParticleSystem muzzleFire;
	public ParticleSystem impact;
	public Transform impactPos;

	public Animator sightAnime;
	public readonly string animeCtrlName = "fireRF";
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

	public override void reload()
	{
		bullet = bulletMax;
		Debug.Log("Reload");
	}

	public override void select()
	{
		if(bullet > 0)
			loaded = true;
		Debug.Log("Select RF");
	}

	private IEnumerator fireAction()
	{
		// 更新AR環境碰撞體，等待3幀
		SingleObj<DepthMeshColliderCus>.instance.ScanDepthCollider();
		yield return new WaitForSeconds(0.1f);

		Vector2 raycastPose = new Vector2(GunControl.gunRay.position.x + (Screen.width / 2), GunControl.gunRay.position.y + (Screen.height / 2));
		GameObject hitEnemy = null;
		muzzleFire.gameObject.SetActive(true);
		muzzleFire.Play();
		impact.gameObject.SetActive(true);
		Ray ray = FirstPersonCamera.ScreenPointToRay(new Vector3(raycastPose.x, raycastPose.y, 0));
		gameSceneFire(ray, shotDistance, (hit) =>
		{
			impactPos.position = hit.point;
			impactPos.LookAt(hit.point - hit.normal);
			impactPos.transform.Translate(Vector3.back * 0.01f);
			if(!impact.isPlaying)
			{
				impact.Play();
			}
			hitEnemy = hit.collider.gameObject;
		}, () => {impact.Stop(); });

		/*if (hitEnemy == null)
		{
			arCoreFire(raycastPose, (hit) =>
			{
				impact.transform.position = hit.Pose.position;
				impact.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.Pose.position);
				impact.Play();
			});
		}*/


		if (loaded)
		{
			hitEnemy?.GetComponent<EnemyRecvDamage>()?.recvDamage(damage * 1.5f);
		}
		else
		{
			hitEnemy?.GetComponent<EnemyRecvDamage>()?.recvDamage(damage);
		}	

		loaded = false;

		sightAnime.SetBool(animeCtrlName, true);

		yield return new WaitForSeconds(0.3f);

		muzzleFire.gameObject.SetActive(false);
		impact.gameObject.SetActive(false);
		sightAnime.SetBool(animeCtrlName, false);

	}
}
