using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSG : GunModel
{
    public float scatterSize = 50f;
    public ParticleSystem muzzleFire;
    public ParticleSystem impact;
    public Transform impactPos;
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
        else if(bullet <= 0)
		{
			//BTsocket.getBTsocket(Constants.bleMicroBit).writeCharacteristic("P0#");
		}

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

        Ray[] rays = scatterRays(new Vector3(raycastPose.x, raycastPose.y, 0));
        foreach (Ray ray in rays)
        {
            gameSceneFire(ray, shotDistance, (hit) =>
            {
                impactPos.position = hit.point;
                impactPos.LookAt(hit.point - hit.normal);
                impactPos.transform.Translate(Vector3.back * 0.01f);
                if (!impact.isPlaying)
                {
                    impact.Play();
                }
                hitEnemy = hit.collider.gameObject;
            }, () => { impact.Stop(); });

            if (loaded)
            {
                hitEnemy?.GetComponent<Enemy>()?.recvDamage(damage * 1.5f);
                hitEnemy?.GetComponent<BoomBox>()?.recvDamage(damage * 1.5f);
                if (Game.mode != Mode.PVE)
                    hitEnemy?.GetComponentInParent<NetworkPlayer>()?.recvDamage(damage * 1.5f);
            }
            else
            {
                hitEnemy?.GetComponent<Enemy>()?.recvDamage(damage);
                hitEnemy?.GetComponent<BoomBox>()?.recvDamage(damage);
                if (Game.mode != Mode.PVE)
                    hitEnemy?.GetComponentInParent<NetworkPlayer>()?.recvDamage(damage);
            }

            //彈孔殘留效果，延遲5秒後消失(請參考ImpactShowDelay.cs)
            if (hitEnemy?.tag == Constants.tagARCollider)
            {
                GameObject impactDelay = impactPool.getObj();
                impactDelay.transform.position = impactPos.position;
                impactDelay.transform.rotation = impactPos.rotation;
            }
        }

        loaded = false;

        sightAnime.SetBool(animeCtrlName, true);

        yield return new WaitForSeconds(0.3f);

        //fireAudio.Stop();
        muzzleFire.gameObject.SetActive(false);
        impact.gameObject.SetActive(false);
        sightAnime.SetBool(animeCtrlName, false);

    }

    private Ray[] scatterRays(Vector3 shotPos)
    {
        Ray[] rays = new Ray[5];
        rays[0] = FirstPersonCamera.ScreenPointToRay(new Vector3(shotPos.x, shotPos.y, 0));
        rays[1] = FirstPersonCamera.ScreenPointToRay(new Vector3(shotPos.x - scatterSize, shotPos.y + scatterSize, 0));
        rays[2] = FirstPersonCamera.ScreenPointToRay(new Vector3(shotPos.x + scatterSize, shotPos.y + scatterSize, 0));
        rays[3] = FirstPersonCamera.ScreenPointToRay(new Vector3(shotPos.x + scatterSize, shotPos.y - scatterSize, 0));
        rays[4] = FirstPersonCamera.ScreenPointToRay(new Vector3(shotPos.x - scatterSize, shotPos.y - scatterSize, 0));
        return rays;
    }

    public override void reload()
    {
        bullet = bulletMax;
        Debug.Log("Reload");
		//BTsocket.getBTsocket(Constants.bleMicroBit).writeCharacteristic("P1#");
        
    }

    public override void select()
    {
        sightAnime.SetBool(animeCtrlName, false);
        if (bullet > 0)
            loaded = true;
        Debug.Log("Select SG");
        fireOK = true;
        /*if(bullet > 0)
			BTsocket.getBTsocket(Constants.bleMicroBit).writeCharacteristic("P1#");
		else
			BTsocket.getBTsocket(Constants.bleMicroBit).writeCharacteristic("P0#");*/

    }
}
