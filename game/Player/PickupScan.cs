using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupScan : MonoBehaviour
{
    [SerializeField]
    private Player player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach(Collider item in Physics.OverlapBox(this.transform.position, new Vector3(1,1,1), Quaternion.identity))
        {
            switch(item.tag)
            {
                case "HPBox":
                    item.GetComponent<HPBox>().pickupAction((health) => {player.recvDamage(-health);});    
                break;
                case "BulletBox":
                    item.GetComponent<BulletBox>().pickupAction((bulletAddVal) => 
                    {
                        GunModel gun = SingleObj<GunManager>.instance.gunMain;
                        gun.bulletMax += (int)(gun.bulletStandSize * bulletAddVal);
                    }); 
                break;
            }
        }
    }

    /*void OnTriggerStay(Collider item)
    {
        Debug.Log(item.tag);
        switch(item.tag)
        {
            case "HPBox":
                item.GetComponent<HPBox>().pickupAction((health) => {player.recvDamage(-health);});
                
            break;
        }
    }*/
}
