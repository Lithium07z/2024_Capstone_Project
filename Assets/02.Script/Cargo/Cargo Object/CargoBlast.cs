using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoBlast : Cargo
{
    public GameObject blastParticle;
    public Material brokenMaterial;
    
    public void Broke()
    {
        base.isBroken = true;
        base._renderer.material = brokenMaterial;
        Destroy(gameObject, base.destroyTime);
    }
    public void Blast()
    {
        Instantiate(blastParticle, transform.position, Quaternion.identity);
        base._rigidbody.constraints = RigidbodyConstraints.None;
        Broke();
    }
}
