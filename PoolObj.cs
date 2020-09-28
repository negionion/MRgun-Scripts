using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObj : MonoBehaviour
{
    private Queue<GameObject> pool = new Queue<GameObject>();
    [SerializeField]
    private GameObject obj;
    public int poolSize = 5;


    // Start is called before the first frame update
    void Start()
    {
        creatObj();
    }

    private void creatObj()
	{
		GameObject _obj;
        for(int i = pool.Count; i < poolSize; i++)
        {
            _obj = Instantiate(obj);
			_obj.SetActive(false);
			_obj.transform.SetParent(gameObject.transform, false);
			pool.Enqueue(_obj);
        }
	}

    public GameObject getObj()
    {
        if (pool.Count == 0)
        {            
            creatObj();
            poolSize *= 2;
        }
		GameObject _reuse = pool.Dequeue();
		_reuse.SetActive(true);                     //顯示物件 
		return _reuse;
    }

    public void recoveryObj(GameObject reObj)    //回收物件
	{
        reObj.SetActive(false);
		pool.Enqueue(reObj);		
	}
}
