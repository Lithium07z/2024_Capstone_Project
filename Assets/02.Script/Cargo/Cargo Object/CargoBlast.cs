using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoBlast : Cargo
{
    public GameObject blastParticle;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            Blast();
        }
    }
    
    public void Blast()
    {
        base.Broke();
        Instantiate(blastParticle, transform.position, Quaternion.identity);
    }
}
