using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EnemyAttack : MonoBehaviour
{
    public float damage = 10;
    public ParticleSystem fireEffect;
    public AudioSource fireSound;

    public void attack()
    {

        fireEffect.Play();
        fireSound.Play();
        
        StartCoroutine(delayShot());

        
    }

    private IEnumerator delayShot()
    {
        SingleObj<DepthMeshColliderCus>.obj.ScanDepthCollider();
		yield return new WaitForSeconds(0.1f);

        RaycastHit hit;
        //Physics.queriesHitBackfaces = true;
        if(Physics.Raycast(fireEffect.transform.position, (SingleObj<ARCoreCtrl>.obj.player.transform.position - fireEffect.transform.position), out hit, 100))
        {
            if(hit.transform.tag == Constants.tagPlayer)
            {
                hit.transform.gameObject.GetComponent<Player>()?.recvDamage(damage);
            }
        }
        //Physics.queriesHitBackfaces = false;

    }
}
