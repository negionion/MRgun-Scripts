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
		else if(bullet <= 0)
		{
			//BTsocket.getBTsocket(Constants.bleMicroBit).writeCharacteristic("S0#");
		}
	}

	public override void reload()
	{
		bullet = bulletMax;
		Debug.Log("Reload");
		//BTsocket.getBTsocket(Constants.bleMicroBit).writeCharacteristic("S1#");
	}

	public override void select()
	{
		sightAnime.SetBool(animeCtrlName, false);
		if(bullet > 0)
			loaded = true;
		Debug.Log("Select RF");
		fireOK = true;
		/*if(bullet > 0)
			BTsocket.getBTsocket(Constants.bleMicroBit).writeCharacteristic("S1#");
		else
			BTsocket.getBTsocket(Constants.bleMicroBit).writeCharacteristic("S0#");*/
	}

	private IEnumerator fireAction()
	{
		// 更新AR環境碰撞體，等待3幀
		SingleObj<DepthMeshColliderCus>.obj.ScanDepthCollider();
		yield return new WaitForSeconds(0.1f);

		Vector2 raycastPose = getSightPosToScreen();
		GameObject hitEnemy = null;
		muzzleFire.gameObject.SetActive(true);
		muzzleFire.Play();
		impact.gameObject.SetActive(true);
		fireAudio.Play();
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
			hitEnemy?.GetComponent<Enemy>()?.recvDamage(damage * 1.5f);
			hitEnemy?.GetComponent<BoomBox>()?.recvDamage(damage * 1.5f);
			if(Game.mode != Mode.PVE)
			hitEnemy?.GetComponentInParent<NetworkPlayer>()?.recvDamage(damage * 1.5f);
		}
		else
		{
			hitEnemy?.GetComponent<Enemy>()?.recvDamage(damage);
			hitEnemy?.GetComponent<BoomBox>()?.recvDamage(damage);
			if(Game.mode != Mode.PVE)
			hitEnemy?.GetComponentInParent<NetworkPlayer>()?.recvDamage(damage);
		}

		//彈孔殘留效果，延遲5秒後消失(請參考ImpactShowDelay.cs)
		if(hitEnemy?.tag == Constants.tagARCollider)
		{
			GameObject impactDelay = impactPool.getObj();
			impactDelay.transform.position = impactPos.position;
			impactDelay.transform.rotation = impactPos.rotation;
		}

		loaded = false;

		sightAnime.SetBool(animeCtrlName, true);

		yield return new WaitForSeconds(0.3f);
		
		//fireAudio.Stop();
		muzzleFire.gameObject.SetActive(false);
		impact.gameObject.SetActive(false);
		sightAnime.SetBool(animeCtrlName, false);

	}
}
