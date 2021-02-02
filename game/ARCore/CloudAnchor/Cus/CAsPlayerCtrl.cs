namespace MRGun.CloudAnchor
{

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Networking;
    using UnityEngine.UI;
    public class CAsPlayerCtrl : NetworkBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            if (!isLocalPlayer)
                return;
            
            gameObject.transform.position = Camera.main.transform.position;
            gameObject.transform.rotation = Camera.main.transform.rotation;
        }
    }
}
