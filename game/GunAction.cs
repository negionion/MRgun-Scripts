using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



public class GunAction : MonoBehaviour
{
	[SerializeField]
	private GunControl gunCtrl = null;

	private bool flagFire = false;

	//武器操作
	public event EventHandler<bool> evtFire;
	public event EventHandler evtReload;
	public event EventHandler<GunType> evtSelect;

	private List<Action> fireActions;

	void Awake()
	{
		gunCtrl.gunStateChangedEvt += OnGunStateChanged;    //掛載Event	
		if(fireActions == null)
			fireActions = new List<Action>();
		evtFire += OnFireAction;	
	}

	public void addFireAction(Action actEvt)
	{
		
		fireActions.Add(actEvt);
	}

	public void rmFireAction(Action actEvt)
	{
		fireActions.Remove(actEvt);
	}

	public void OnFireAction(object sender, bool isFire)
	{
		if(isFire && fireActions.Count > 0)
		{
			foreach(var fireEvt in fireActions)
			{
				fireEvt();
			}
		}
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
