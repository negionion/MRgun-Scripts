using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBox : MonoBehaviour
{
    public float addBullet = 0.2f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void pickupAction(System.Action<float> pickupAction)
    {
        pickupAction(addBullet);
        #if UNITY_ANDROID
        Handheld.Vibrate();
        #endif
        Destroy(gameObject);
    }
}
