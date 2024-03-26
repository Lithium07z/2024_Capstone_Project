using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Cargo : MonoBehaviour
{
    public CargoData cargoData;

    public GameObject blastParticle;
    
    public Material brokenMaterial;
    
    private MeshRenderer _renderer;
    private Rigidbody _rigidbody;

    private void Start()
    {
        
        _renderer = transform.GetChild(0).GetComponent<MeshRenderer>();
        _rigidbody = GetComponent<Rigidbody>();
        
        if(_renderer is null) { Debug.LogWarning("에셋 구조가 잘못 됨."); }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            Blast();
        }
    }

    public void Broke()
    {
        cargoData.isBroken = true;
        _renderer.material = brokenMaterial;
        Destroy(gameObject, cargoData.destroyTime);
    }
    public void Blast()
    {
        Instantiate(blastParticle, transform.position, Quaternion.identity);
        Broke();
    }
}
