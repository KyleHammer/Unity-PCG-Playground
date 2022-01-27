using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalizedDirection : MonoBehaviour
{
    public Vector3 direction = new Vector3();
    public Vector3 endPos = Vector3.one;

    private void Update()
    {
        
        //direction = endPos - transform.position;
        //direction = direction.normalized;
        
        Debug.DrawRay(transform.position, direction * 5, Color.red, Time.deltaTime);
    }
}
