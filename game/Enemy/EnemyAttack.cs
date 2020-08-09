﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public float damage = 10;
    public ParticleSystem fireEffect;
    public AudioSource fireSound;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void attack()
    {
        fireEffect.Play();
        fireSound.Play();
        RaycastHit hit;
        if(Physics.Raycast(transform.position, Camera.main.transform.position - transform.position, out hit, 100))
        {
            if(hit.transform.tag == Constants.tagPlayer)
            {
                hit.transform.gameObject.GetComponent<Player>()?.recvDamage(damage);
            }
        }
    }
}