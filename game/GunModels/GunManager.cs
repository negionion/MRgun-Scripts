using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GunManager : MonoBehaviour
{
	[SerializeField]
	private GunModel gunAR = null, gunSG = null, gunRF = null;

	public GunModel gunMain;
	[SerializeField]
	private GunAction gunAction = null;

	// Start is called before the first frame update
	void Awake()
    {
		gunAction.evtFire += OnFire;
		gunAction.evtSelect += OnSelect;
		gunAction.evtReload += OnReload;		
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	private void OnFire(object sender, bool isFire)
	{
		gunMain.fire(isFire);
	}

	private void OnSelect(object sender, GunType gunType)
	{
		gunMain.fire(false);
		gunMain.gameObject.SetActive(false);
		switch (gunType)
		{
			case GunType.AR:
				gunMain = gunAR;
				break;
			case GunType.RF:
				gunMain = gunRF;
				break;
			case GunType.SG:
				gunMain = gunSG;
				break;
			default:
				gunMain = gunAR;
				break;
		}
		//initial
		gunMain.gameObject.SetActive(true);
		gunMain.select();
	}
	private void OnReload(object sender, EventArgs e)
	{
		gunMain.reload();
	}
}
