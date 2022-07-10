using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.Networking;
using System;

public enum EnemyState
{
    INI,
    STAY,
    DIE
}

public class Enemy : NetworkBehaviour
{
    public EnemyState state {private set; get;}
    [SerializeField]
    public float delayActiveTime = 2f;
	public int hpMax = 100;
	public int hp = 100;
    [SerializeField]
    private Image hpBar = null;
    public UnityEvent onInitial;
    public UnityEvent onHurt;
    public UnityEvent onDie;
    public UnityEvent onAttack;
    private bool effectFlag = false;

	public Transform targetPos;
    public bool isLocal{private set; get;} = false;

	// Start is called before the first frame update
	void Start()
    {
        gameObject.tag = Constants.tagEnemy;
        onInitial.Invoke();

        InvokeRepeating("attackAction", 5f, 3f);
        Invoke("onDie", 30f);
                
        state = EnemyState.INI;

        //StartCoroutine(delayStart());
        scanTargetPlayer();

        
    }

    // Update is called once per frame
    void Update()
    {
        if(targetPos == null)
        {            
            //目標改為房主
            targetPos = Player.netPlayers[0].transform;
        }
        transform.LookAt(targetPos.position);

        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        if(hpBar != null && state == EnemyState.STAY)
        {
            hpBar.gameObject.GetComponentInParent<Canvas>().transform.rotation = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0);
        }
    }

    private void scanTargetPlayer()
    {
        //Debug.LogWarning("player account："+GameObject.FindGameObjectsWithTag(Constants.tagNetPlayer).Length);
        foreach(NetworkPlayer _netPlayer in Player.netPlayers)
        {
            if(_netPlayer.GetComponent<NetworkIdentity>() == null)
                continue;

            //GameObject.Find("CAsDebugText").GetComponent<UnityEngine.UI.Text>().text += _netPlayer.name + " id = " + _netPlayer.GetComponent<NetworkIdentity>().netId + "\n";
            if(_netPlayer.GetComponent<NetworkIdentity>()?.netId.Value == GetComponent<EnemyMultiplayerCtrl>().targetId)
            {
                
                targetPos = _netPlayer.transform;
                GameObject.Find("CAsDebugText").GetComponent<UnityEngine.UI.Text>().text += "targetId = " + _netPlayer.GetComponent<NetworkIdentity>().netId.Value.ToString() + "\n";
                if(targetPos.name == Constants.nameLocalPlayer)     //本地端生成，才需要Spawn到其他客戶端
                {
                    StartCoroutine(delaySpawn());
                }
                else
                {
                    GetComponent<ProjectileCus>().closeAutoCalibrate();
                }
                Debug.Log(targetPos.name);

                break;
            }            
        }
    }

    private IEnumerator delaySpawn()        //在其他客戶端延遲生成
    {
        while (!GetComponent<ProjectileCus>().isSetted)      //物理運算未結束，座標未確定
        {
            yield return 0;            
        }
        //敵人出現自然下落的物理運算結束，在其他客戶端中生成敵人
        EnemyGenerator.enemys.Add(this);
        //GameObject.Find("CAsDebugText").GetComponent<UnityEngine.UI.Text>().text += "enemy = " + EnemyGenerator.enemys.IndexOf(this) + "\n";
        GameObject.Find(Constants.nameLocalPlayer).GetComponent<MultiplayerCtrl>().
		    CmdSpawnEnemy(transform.position, transform.rotation, GetComponent<EnemyMultiplayerCtrl>().targetId);
        
    }

    public void delayActive(GameObject gobj)
    {
        StartCoroutine(delayActiveInvoke(gobj));
    }
    private IEnumerator delayActiveInvoke(GameObject gobj)
    {
        yield return new WaitForSeconds(delayActiveTime);
        gobj.SetActive(true);
        state = EnemyState.STAY;

    }

    private void attackAction()
    {
        if(state == EnemyState.STAY && GetComponent<ProjectileCus>().isSetted)
            onAttack.Invoke();
        
    }

    public void recvDamage(float damage)
	{
        Debug.LogWarning("Enemy Hit!" + EnemyGenerator.enemys.IndexOf(this));
        if(state == EnemyState.STAY)
        {            
            //hp -= (int)damage;
            
            MultiplayerCtrl.getLocalPlayer().GetComponent<MultiplayerCtrl>().CmdRecvDamageEnemy(EnemyGenerator.enemys.IndexOf(this), damage);
            
        }		
	}

    public void syncRecvDamage(int _hp)
	{
        hp = _hp;
        if(state == EnemyState.STAY)
        {            
            //Debug.LogWarning("sync damage");
            
            hpBar.fillAmount = (float)hp / hpMax;
            hpBar.GetComponent<HPBarCtrl>()?.showHP();	
            onHurt.Invoke();
            if(hp <= 0)
            {
                die();
                return;
            }
        }
        
	}

    private void die()
	{
		Debug.Log("GG");
        EnemyGenerator.enemys.Remove(this);   
        onDie.Invoke();
        SingleObj<ItemGenerator>.obj.itemGenerate(this.transform);
        state = EnemyState.DIE;
        
        //gameObject.GetComponent<Collider>().enabled = false;
        Destroy(gameObject, 5f);
		//gameObject.GetComponent<MeshRenderer>().material.color = Color.black;
		//gameObject.SetActive(false);
	}
}
