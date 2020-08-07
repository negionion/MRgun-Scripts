using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRecvDamage : MonoBehaviour
{
	private Enemy enemy;
	public float hitDelay = 1f;
	private float hitTiming = 0;
	private bool effectFlag = false;
    // Start is called before the first frame update
    void Start()
    {
		enemy = GetComponent<Enemy>();
		enemy.hp = enemy.hpMax;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void recvDamage(float damage)
	{
		enemy.hp -= (int)damage;
		hitTiming = 0;
		StartCoroutine(hurtEffect());
		if(enemy.hp <= 0)
		{
			die();
		}
	}

	private IEnumerator hurtEffect()
	{
		if(effectFlag)
			yield break;
		effectFlag = true;
		while(hitTiming <= hitDelay)
		{
			gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
			hitTiming += Time.deltaTime;
			yield return 0;
		}
		gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
		effectFlag = false;
	}

	private void die()
	{
		Debug.Log("GG");
		gameObject.GetComponent<MeshRenderer>().material.color = Color.black;
		gameObject.SetActive(false);
	}
}
