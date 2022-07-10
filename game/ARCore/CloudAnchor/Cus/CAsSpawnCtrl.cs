using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CAsSpawnCtrl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {

            var spawnCtrl = GameObject.Find("LocalPlayer").GetComponent<MRGun.CloudAnchor.CAsShootTest>();
            spawnCtrl.GetComponent<MRGun.CloudAnchor.CAsShootTest>().CmdSpawnTest(spawnCtrl.netId.Value);


            //GameObject.Find("LocalPlayer").GetComponent<MRGun.CloudAnchor.LocalPlayerController>().CmdSpawnStar(Vector3.zero, Quaternion.identity);
        }
    }
}
