    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Blast : MonoBehaviour
{
    public float power;
    public float upperPower;
    public float range;

    public float damage;


    public LayerMask layermask;
    

    private Rigidbody _rigidbody;

    void Awake()
    {
        //layermask = ??
        _rigidbody = GetComponent<Rigidbody>();
    }
    
    void Start()
    {
        BlastRigidbody();
        Destroy(gameObject, 3f);
    }

    void BlastRigidbody()
    {
        Collider[] colliders;
        colliders = Physics.OverlapSphere(transform.position, range);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject.layer == layermask)
            {
                _rigidbody.AddExplosionForce(power, transform.position, range, upperPower);
                Damage(colliders[i]?.GetComponent<PlayerProperty>());
            }
        }
    }

    void Damage(PlayerProperty player)
    {
        if(player is not null) player.TakeDamage(damage); 
    }
}
