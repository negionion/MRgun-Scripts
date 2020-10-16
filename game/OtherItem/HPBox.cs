using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBox : MonoBehaviour
{
    [SerializeField]
    private int healthPoint = 20;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void pickupAction(System.Action<int> pickupAction)
    {
        pickupAction(healthPoint);
        Destroy(gameObject);
    }


}
