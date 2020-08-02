using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunUI : MonoBehaviour
{
	private GunModel gun;

	public Text bullet;

    // Start is called before the first frame update
    void Start()
    {
		
    }

    // Update is called once per frame
    void Update()
    {

		gun = SingleObj<GunManager>.instance.gunMain;

		bullet.text = string.Format("{2}\n{0}/{1}", gun.bullet, gun.bulletMax, gun.ToString().Substring(3, 2));

    }
}
