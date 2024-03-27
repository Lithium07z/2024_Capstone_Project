using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using Photon.Pun;

public class Cargo : MonoBehaviour
{
    public CargoData cargoData;
    public Material brokenMaterial;
    protected PhotonView photonview;

    private string cargoName;
    private float value;
    private float weight;

    private bool isBroken = false;
    public float destroyTime;
    
    [SerializeField] private MeshRenderer _renderer;
    private Rigidbody _rigidbody;

    public void SetData(string name, float value, float weight)
    {
        cargoName = name;
        this.value = value;
        this.weight = weight;
    }

    protected virtual void Start()
    {
        photonview = GetComponent<PhotonView>();
        _renderer = transform.GetChild(0).GetComponent<MeshRenderer>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    protected void Broke()
    {
        isBroken = true;
        value = 0f;
        _renderer.material = brokenMaterial;
        Destroy(gameObject, destroyTime);
    }
}
