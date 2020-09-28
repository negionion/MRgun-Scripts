using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactShowDelay : MonoBehaviour
{
    public float delay = 5f;
    // Start is called before the first frame update
    void OnEnable()
    {
        Invoke("delayRecovery", delay);
    }

    private void delayRecovery()
    {
        GetComponentInParent<PoolObj>().recoveryObj(gameObject);
    }
}
