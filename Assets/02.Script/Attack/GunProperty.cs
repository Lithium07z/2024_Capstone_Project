using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GunProperty : MonoBehaviour
{
    public GameObject leftIKPivot;

    private void Update()
    {
        Debug.Log("position from GunProperty: " + leftIKPivot.transform.position);
    }
}
