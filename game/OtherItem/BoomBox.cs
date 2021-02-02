﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BoomBox : MonoBehaviour
{
    public int boomDamage = 20;
    public int hp = 50;
    public float size = 1.5f;
    public UnityEvent onBoom;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("scanBoom", 3f, 1f);
    }

    private void scanBoom()
    {
        foreach(Collider item in Physics.OverlapBox(this.transform.position, (new Vector3(1,1,1)) * 2, Quaternion.identity))
        {
            if(Constants.tagPlayer == item.tag)
            {
                item.GetComponent<Player>().recvDamage(boomDamage);
                boom();
            }
        }
    }

    public void recvDamage(float damage)
	{
                    
        hp -= (int)damage;	
        if(hp <= 0)
        {
            foreach(Collider item in Physics.OverlapBox(this.transform.position, (new Vector3(1,1,1)) * 2, Quaternion.identity))
            {
                switch(item.tag)
                {
                    case Constants.tagEnemy:
                        item.GetComponent<Enemy>().recvDamage(boomDamage * 3);
                    break;
                    case Constants.tagPlayer:
                        item.GetComponent<Player>().recvDamage(boomDamage);
                    break;
                }
            }
            boom();
            return;
        }
        
        
		//StartCoroutine(hurtEffect());
		
	}

    public void boom()
    {
        CancelInvoke();
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        gameObject.GetComponent<Collider>().enabled = false;
        gameObject.GetComponentInChildren<SpriteRenderer>().enabled = false;
        onBoom?.Invoke();
        this.enabled = false;

        Destroy(gameObject, 3f);
    }
}
