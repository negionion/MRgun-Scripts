using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SpatialTracking;

public class Player : MonoBehaviour
{
    public static List<NetworkPlayer> netPlayers = new List<NetworkPlayer>();
    public int hpMax = 100;
	public int hp = 100;
    public Image hpBar;
    public RawImage hitEffect;
    // Start is called before the first frame update

    void Start()
    {
        gameObject.tag = Constants.tagPlayer;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void recvDamage(float damage)            //HP 控制，給負值可回血
    {
        if(hp > 0)
        {
            hp -= (int)damage;
            hpBar.fillAmount = (float)hp / hpMax;
            hitEffect.GetComponent<Animation>().Play();
            #if UNITY_ANDROID
            //Handheld.Vibrate();   //手機震動效果
            #endif
            
        }
        if(hp > hpMax)
            hp = hpMax;
            
        GameObject.Find(Constants.nameLocalPlayer).GetComponent<NetworkPlayer>().CmdUpdatePlayerHP(damage);
    } 

}
