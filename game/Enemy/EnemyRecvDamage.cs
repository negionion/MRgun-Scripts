using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRecvDamage : MonoBehaviour
{
	Enemy enemy;
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
		if(enemy.hp == 0)
		{
			die();
		}
	}

	private void die()
	{
		Debug.Log("GG");
		gameObject.SetActive(false);
	}
}
