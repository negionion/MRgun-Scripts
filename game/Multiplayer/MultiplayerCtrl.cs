using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class MultiplayerCtrl : NetworkBehaviour
{

    public static bool isConnected = false;

    public GameObject enemyPrefab;

    private GameObject mainPlayer;

    // Start is called before the first frame update
    public override void OnStartLocalPlayer()
    {

        base.OnStartLocalPlayer();

        gameObject.name = Constants.nameLocalPlayer;
        mainPlayer = SingleObj<ARCoreCtrl>.obj.player;    //取得客戶端本地運算的Player
        GameObject.Find("CAsDebugText").GetComponent<UnityEngine.UI.Text>().text = "localId = " + getLocalNetId() + "\n";
    
    }

    public void Update()
    {
        if (!isLocalPlayer)
                return;
            
        gameObject.transform.position = mainPlayer.transform.position;
        gameObject.transform.rotation = Quaternion.Euler(0, mainPlayer.transform.rotation.eulerAngles.y, 0);
    }

    public static uint getLocalNetId()
    {
        try
        {
            return GameObject.Find(Constants.nameLocalPlayer).GetComponent<NetworkIdentity>().netId.Value;
        }
        catch (System.NullReferenceException e)
        {return Constants.nullNetId;}

    }

    public static GameObject getLocalPlayer()
    {
        try
        {
            return GameObject.Find(Constants.nameLocalPlayer);
        }
        catch (System.NullReferenceException e)
        {return null;}

    }

    [Command]
    public void CmdRecvDamageEnemy(int enemyIndex, float damage)
    {
        if(EnemyGenerator.enemys[enemyIndex].state == EnemyState.STAY)
        {            
            EnemyGenerator.enemys[enemyIndex].hp -= (int)damage;
            RpcRecvDamageEnemy(enemyIndex, EnemyGenerator.enemys[enemyIndex].hp);
        }
    }
    
    [ClientRpc]
    public void RpcRecvDamageEnemy(int enemyIndex, int _hp)
    {
        EnemyGenerator.enemys[enemyIndex].syncRecvDamage(_hp);
    }
    

    [Command]
    public void CmdSpawnEnemy(Vector3 _pos, Quaternion _rotate, uint _targetId)
    {
        Debug.LogWarning("Cmd !!");
        RpcSpawnEnemy(_pos, _rotate, _targetId);       
    }

    [ClientRpc]
    public void RpcSpawnEnemy(Vector3 _pos, Quaternion _rotate, uint _targetId)
    {
        
        if(_targetId == MultiplayerCtrl.getLocalNetId()) //代表是自己產生的敵人，不再重複生成
        {
            return;
        }
        if(EnemyGenerator.enemys.Count == SingleObj<EnemyGenerator>.obj.enemyMaxCount)     //取得client獨立的怪物產生器，檢查怪物是否超過max數量
        {
            var delEnemy = EnemyGenerator.enemys[0];
            EnemyGenerator.enemys.RemoveAt(0);
            Destroy(delEnemy.gameObject);  //移除最早出現的敵人
        }
        var spawnEnemy = Instantiate(enemyPrefab, _pos, _rotate);             //client生成敵人，新敵人加入Queue中
        EnemyGenerator.enemys.Add(spawnEnemy.GetComponent<Enemy>());
        GameObject.Find("CAsDebugText").GetComponent<UnityEngine.UI.Text>().text += "enemy = " + EnemyGenerator.enemys.IndexOf(spawnEnemy.GetComponent<Enemy>()) + "\n";
        spawnEnemy.GetComponent<EnemyMultiplayerCtrl>().targetId = _targetId;
        
    }
}
