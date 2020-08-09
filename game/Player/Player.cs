using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int hpMax = 100;
	public int hp = 100;

    public Image hpBar;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.tag = Constants.tagPlayer;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void recvDamage(float damage)
    {
        if(hp > 0)
        {
            hp -= (int)damage;
            hpBar.fillAmount = (float)hp / hpMax;
            Handheld.Vibrate();	
        }
    }
}
