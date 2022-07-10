using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBarCtrl : MonoBehaviour
{
    public float showTime = 2f;
    private float showTiming = 0;
    private Image hpBar;
    private Color m_color;
    // Start is called before the first frame update
    void Start()
    {
        hpBar = gameObject.GetComponent<Image>();
        m_color = hpBar.color;
    }

    // Update is called once per frame
    void Update()
    {
        
        if(showTiming <= 0)
        {
            m_color.a = 0;
            hpBar.color = m_color;
        }
        else
        {
            showTiming -= Time.deltaTime;
        }
    }

    public void showHP(float time = 0)  //若無填入任何參數，則直接使用預設值，否則更新預設值
    {
        if(time > 0)
        {
            showTime = time;
        }        
        showTiming = showTime;
        m_color.a = 255;
        hpBar.color = m_color;
    }
}
