using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
	public int hpMax = 100;
	public int hp;

    [SerializeField]
    private Image hpBar;
	// Start is called before the first frame update
	void Start()
    {
        gameObject.tag = "Enemy";
    }

    // Update is called once per frame
    void Update()
    {
        if(hpBar != null)
        {
            hpBar.gameObject.GetComponentInParent<Canvas>().transform.rotation = Camera.main.transform.rotation;
            hpBar.fillAmount = (float)hp / hpMax;
        }
    }
}
