using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator : MonoBehaviour
{
    public GameObject[] gameItems;
    private int cnt = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void itemGenerate(Transform pos)
    {
        int randomVal = cnt;
        cnt++;
        if(cnt >= gameItems.Length)
            cnt = 0;
        //int randomVal = Random.Range(0, (int)(gameItems.Length));       //調整方塊出現機率(目前100%平分3種)
        if(randomVal < gameItems.Length)
        {
            GameObject obj = Instantiate(gameItems[randomVal], pos);
            obj.transform.SetParent(this.transform);
        }
    }
}
