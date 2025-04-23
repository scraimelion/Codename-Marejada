using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    void Update() 
    {
        Vector3 look = Camera.main.transform.position;
        look.y = transform.position.y;
        transform.LookAt( look, Vector3.up);
    }
}

