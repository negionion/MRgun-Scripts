using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum GunType
{
	AR,
	RF,
	SG,
	EMPTY
}

public class GunAction : MonoBehaviour
{
	[SerializeField]
	private GunControl gunCtrl = null;

	private bool flagFire = false;

	//武器操作
	public event EventHandler<bool> evtFire;
	public event EventHandler evtReload;
	public event EventHandler<GunType> evtSelect;

	void Awake()
	{
		gunCtrl.gunStateChangedEvt += OnGunStateChanged;    //掛載Event		
	}

    void Start()
	{
		evtSelect(this, GunType.AR);                        //預設裝備AR
	}

	private void OnGunStateChanged(object sender, char state)
	{
		switch (state)
		{
			case '@':
				flagFire = true;
				evtFire(this, true);				
				break;
			case 'R':
				if (!flagFire)
					evtReload(this, EventArgs.Empty);
				else
				{
					flagFire = false;
					evtFire(this, false);
				}
				break;
			case 'P':
				evtSelect(this, GunType.SG);
				break;
			case 'S':
				evtSelect(this, GunType.RF);
				break;
			case 'A':
				evtSelect(this, GunType.AR);
				break;
			default:
				flagFire = false;
				evtFire(this, false);
				break;
		}
	}
}
