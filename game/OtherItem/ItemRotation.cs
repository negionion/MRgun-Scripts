using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRotation : MonoBehaviour
{
    void Update()
    {
        gameObject.transform.RotateAround(transform.position, Vector3.up, 360 * Time.deltaTime * 0.2f);
    }
}
