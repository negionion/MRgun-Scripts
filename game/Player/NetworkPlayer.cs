using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class NetworkPlayer : NetworkBehaviour
{
    [SerializeField]
    private Image hpBar;

    public GameObject thisPlayerTrigger;
    private Player player;
    private int hp, hpMax;


    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        player = SingleObj<Player>.obj;
        hp = player.hp;
        hpMax = player.hpMax;
        CmdIniPlayerHP(hp, hpMax);  //在所有客戶端的初始化自己的血量
        Destroy(hpBar.gameObject);
        Destroy(GetComponentInChildren<Collider>().gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        Player.netPlayers.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        if(isLocalPlayer || thisPlayerTrigger == null)  //以下內容僅在其他客戶端中運行
        {
            return;
        }
        thisPlayerTrigger.transform.rotation = gameObject.transform.rotation;
        //thisPlayerTrigger.transform.LookAt(SingleObj<Player>.obj.transform);
    }

    void OnDestroy()
    {
        Player.netPlayers.Remove(this);
    }

    public void recvDamage(float damage)
    {
        GameObject.Find(Constants.nameLocalPlayer).GetComponent<NetworkPlayer>().CmdTakeDamageToNetPlayer(GetComponent<NetworkIdentity>().netId.Value, damage);
    }

    [Command]
    public void CmdTakeDamageToNetPlayer(uint _netId, float damage)
    {
        foreach (NetworkPlayer _netPlayer in Player.netPlayers)
        {
            if (_netPlayer.GetComponent<NetworkIdentity>().netId.Value == _netId)
            {
                GameObject.Find("CAsDebugText").GetComponent<UnityEngine.UI.Text>().text += "player = " + _netId + "damage = " + damage + "\n";
                TargetDoRecvDamage(_netPlayer.GetComponent<NetworkIdentity>().connectionToClient, damage);
                //_netPlayer.CmdUpdatePlayerHP(damage);
            }
        }
    }

    [TargetRpc]
    public void TargetDoRecvDamage(NetworkConnection connection, float damage)
    {
        //GameObject.Find("CAsDebugText").GetComponent<UnityEngine.UI.Text>().text += connection.ToString() + " damage = " + damage + "\n";
        SingleObj<Player>.obj.recvDamage(damage);
    }
    
    [Command]
    public void CmdIniPlayerHP(int _hp, int _hpMax)
    {
        hp = _hp;
        hpMax = _hpMax;
        CmdUpdatePlayerHP(0);
    }

    [Command]
    public void CmdUpdatePlayerHP(float damage)
    {
        if (hp > 0)
        {
            hp -= (int)damage;
        }
        if (hp > hpMax)
            hp = hpMax;

        RpcNetPlayerHPBar(hp, hpMax);

    }

    [ClientRpc]
    public void RpcNetPlayerHPBar(int _hp, int _hpMax)          //同步玩家的血量
    {
        if (!isLocalPlayer)
        {
            hpBar.fillAmount = (float)_hp / _hpMax;
            hpBar.GetComponent<HPBarCtrl>()?.showHP();
        }
    }
}
