using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunUI : MonoBehaviour
{
    private GunModel gun;
    private GunType mainGunType;

    public Text bullet;
    //[SerializeField]
    //private GamePlayer player;
    //public Image hpBar;

    public Sprite[] gunSprites;
    public Image gunModelImage;

    // Start is called before the first frame update
    void Start()
    {
        SingleObj<GunAction>.obj.evtSelect += OnSelect;
    }

    // Update is called once per frame
    void Update()
    {

        gun = SingleObj<GunManager>.obj.gunMain;

        bullet.text = string.Format("{0}/{1}", gun.bullet, gun.bulletMax);

    }

    private void OnSelect(object sender, GunType gunType)
	{
		mainGunType = gunType;
        gunModelImage.sprite = gunSprites[(int)mainGunType];
	}
}
