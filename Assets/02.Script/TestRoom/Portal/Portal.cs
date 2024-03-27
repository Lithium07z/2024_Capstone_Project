using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Portal linkedPortal;
    private Transform warpDestination;

    void Start()
    {
        warpDestination = transform.GetChild(0);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("충돌 오브젝트: " + other.name);
            other.transform.position = linkedPortal.warpDestination.position;
            Debug.Log("이동 완료");
        }
    }
}
