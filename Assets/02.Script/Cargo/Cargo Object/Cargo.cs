using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Cargo : MonoBehaviour
{
    public CargoData cargoData;
    public Material brokenMaterial;

    private string cargoName;
    private float value;
    private float weight;

    private bool isBroken = false;
    public float destroyTime;
    
    private MeshRenderer _renderer;
    private Rigidbody _rigidbody;

    public void SetData(string name, float value, float weight)
    {
        cargoName = name;
        this.value = value;
        this.weight = weight;
    }

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

    protected void Broke()
    {
        isBroken = true;
        value = 0f;
        _renderer.material = brokenMaterial;
        Destroy(gameObject, destroyTime);
    }
}
