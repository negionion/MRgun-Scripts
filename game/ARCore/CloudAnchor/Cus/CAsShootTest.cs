namespace MRGun.CloudAnchor
{
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CAsShootTest : MonoBehaviour
{
    public Text showPos;
    Ray ray;
    RaycastHit shotHit;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {       
        ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));
		if(Physics.Raycast(ray, out shotHit, 10))
		{
			if(shotHit.collider.tag == "StarTest")
            {
                showPos.text = shotHit.transform.position.ToString();
            }
		}
    }
}
}
