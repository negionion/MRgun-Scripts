using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupScan : MonoBehaviour
{
    [SerializeField]
    private Player player;
    public Vector3 scanSize = new Vector3(1f, 1.5f, 1f);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach(Collider item in Physics.OverlapBox(this.transform.position, scanSize, Quaternion.identity))
        {
            switch(item.tag)
            {
                case "HPBox":
                    item.GetComponent<HPBox>().pickupAction((health) => {player.recvDamage(-health);});    
                break;
                case "BulletBox":
                    item.GetComponent<BulletBox>().pickupAction((bulletAddVal) => 
                    {
                        GunModel gun = SingleObj<GunManager>.obj.gunMain;
                        gun.bulletMax += (int)(gun.bulletStandSize * bulletAddVal);
                    }); 
                break;
            }
        }
    }
}
