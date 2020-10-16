using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator : MonoBehaviour
{
    public GameObject[] gameItems;
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
        int randomVal = Random.Range(0, (int)(gameItems.Length * 1.5f));
        if(randomVal < gameItems.Length)
        {
            GameObject obj = Instantiate(gameItems[randomVal], pos);
            obj.transform.SetParent(this.transform);
        }
    }
}
