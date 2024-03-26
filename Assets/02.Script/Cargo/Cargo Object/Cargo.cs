using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Cargo : MonoBehaviour
{
    public CargoData cargoData;

    private string cargoName;
    private float value;
    private float weight;

    protected bool isBroken = false;
    protected float destroyTime;
    
    protected MeshRenderer _renderer;
    protected Rigidbody _rigidbody;

    private void Start()
    {
        _renderer = transform.GetChild(0).GetComponent<MeshRenderer>();
        _rigidbody = GetComponent<Rigidbody>();
        
        
        if(_renderer is null) { Debug.LogWarning("에셋 구조가 잘못 됨."); }

        if (cargoData.currentProperty == CargoData.Fragile.Bomb)
        {
            gameObject.AddComponent<CargoBlast>();
        }
    }
}
