using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public enum EnemyState
{
    INI,
    STAY,
    DIE
}

public class Enemy : MonoBehaviour
{
    public EnemyState state {private set; get;}
	public int hpMax = 100;
	public int hp = 100;
    [SerializeField]
    private Image hpBar;
    public UnityEvent onInitial;
    public UnityEvent onHurt;
    public UnityEvent onDie;
    public UnityEvent onAttack;
    private bool effectFlag = false;
    public float hitDelay = 1f;
	private float hitTiming = 0;
	
	// Start is called before the first frame update
	void Start()
    {
        gameObject.tag = "Enemy";
        onInitial.Invoke();
        InvokeRepeating("attackAction", 5f, 3f);
        Invoke("onDie", 30f);
        state = EnemyState.INI;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Camera.main.transform);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        if(hpBar != null)
        {
            hpBar.gameObject.GetComponentInParent<Canvas>().transform.rotation = Camera.main.transform.rotation;
            hpBar.fillAmount = (float)hp / hpMax;
        }
    }

    private void attackAction()
    {
        
        onAttack.Invoke();
        state = EnemyState.STAY;
        
    }

    public void recvDamage(float damage)
	{
        if(state != EnemyState.DIE)
        {            
            hp -= (int)damage;		
            onHurt.Invoke();
            hitTiming = 0;
            state = EnemyState.STAY;
            if(hp <= 0)
            {
                die();
                return;
            }
        }
        
		//StartCoroutine(hurtEffect());
		
	}

    
	private IEnumerator hurtEffect()
	{
		if(effectFlag)
			yield break;
		effectFlag = true;
		while(hitTiming <= hitDelay)
		{
			gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
			hitTiming += Time.deltaTime;
			yield return 0;
		}
		gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
		effectFlag = false;
	}
    private void die()
	{
		Debug.Log("GG");        
        onDie.Invoke();
        state = EnemyState.DIE;
        //gameObject.GetComponent<Collider>().enabled = false;
        Destroy(gameObject, 5f);
		//gameObject.GetComponent<MeshRenderer>().material.color = Color.black;
		//gameObject.SetActive(false);
	}
}
