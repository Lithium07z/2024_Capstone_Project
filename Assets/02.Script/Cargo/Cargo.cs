using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cargo : MonoBehaviour
{
    public CargoData cargoData;
    public Material brokenMaterial;

    public float destroyTime;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            Broke();
        }
    }

    public void Broke()
    {
        cargoData.isBroken = true;
        GetComponent<MeshRenderer>().material = brokenMaterial;
        Destroy(gameObject, cargoData.destroyTime);
    }
    public void Blast()
    {
        // Create Blast Particle (It has Damage Logic & Area, Sound)
        Broke();
    }
}
