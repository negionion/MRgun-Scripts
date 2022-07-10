namespace MRGun.CloudAnchor
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.Networking;

    public class CAsShootTest : NetworkBehaviour
    {
        public Text showPos;
        Ray ray;
        RaycastHit shotHit;


        public GameObject testObj;
        // Start is called before the first frame update
        void Start()
        {
            showPos = GameObject.Find("Pos").GetComponent<Text>();
        }

        // Update is called once per frame
        void Update()
        {
            ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));
            if (Physics.Raycast(ray, out shotHit, 10))
            {
                if (shotHit.collider.tag == Constants.tagEnemy)
                {
                    
                    //showPos.text = shotHit.collider.GetComponentInParent<SpawnTest>().srcId.ToString();
                }
            }



        }
#pragma warning disable 618
        [Command]
#pragma warning disable 618
        public void CmdSpawnTest(uint id)
        {
            Debug.Log("AAAAAAAAAAAA");
            // Instantiate Star model at the hit pose.
            var spawnObj = Instantiate(testObj, Vector3.zero, Quaternion.identity);
            spawnObj.GetComponent<SpawnTest>().srcId = id;

            // Spawn the object in all clients.
#pragma warning disable 618
            NetworkServer.Spawn(spawnObj);
#pragma warning restore 618
        }
    }
}
