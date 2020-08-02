using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGunCtrl : MonoBehaviour
{
	public int x, y; 
	public char gunSt; 
	public float angle;
	public GunControl gunControl;

    // Update is called once per frame
    void Update()
    {
		gunControl.gunMotionCtrl(new GunControl.MotionData(x, y, angle, gunSt));
    }
}
