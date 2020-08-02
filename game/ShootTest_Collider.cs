using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootTest_Collider : MonoBehaviour
{
    public GunAction gunAction = null;
    public GameObject shootObj;
    
    public UnityEngine.Events.UnityEvent fireEvt;
    // Start is called before the first frame update
    void Start()
    {
        gunAction.evtFire += OnFire;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnFire(object sender, bool isFire)
	{
		if(isFire)
            fireEvt?.Invoke();
	}

    public void shoot()
    {
        GameObject bullet = Instantiate(
            shootObj,
            Camera.main.transform.position + (Camera.main.transform.forward * 0.25f),
            Quaternion.identity) as GameObject;

        Vector3 forceVector = Camera.main.transform.forward * 10f;
        bullet.GetComponent<Rigidbody>().velocity = forceVector;
        bullet.transform.parent = transform;
    }
}
