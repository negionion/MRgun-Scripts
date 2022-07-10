using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using GoogleARCore.CrossPlatform;

public class MultiplayerPCACtrl : NetworkBehaviour
{
    [Command]
    public void CmdSyncPCA(string _cloudID)
    {
        RpcSyncPCA(_cloudID);
    }

    [ClientRpc]
    public void RpcSyncPCA(string _cloudID)
    {
        SingleObj<PersistentCloudAnchorsCtrl>.obj.cancelResolvePCA(_cloudID);
    }
}
