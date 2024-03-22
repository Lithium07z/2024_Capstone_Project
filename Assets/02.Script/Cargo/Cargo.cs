using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cargo : MonoBehaviour
{
    public CargoData cargoData;

    public void Blast()
    {
        // Create Blast Particle (It has Damage Logic & Area, Sound)
        Destroy(gameObject);
    }
}
