using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleObj<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T _obj;

	public static T obj
	{
		get
		{
			_obj = (T)FindObjectOfType(typeof(T));
			if (_obj == null)
			{
				GameObject obj = new GameObject();
				_obj = obj.AddComponent<T>();
				obj.name = typeof(T).ToString();
			}
			return _obj;
		}
	}
}
