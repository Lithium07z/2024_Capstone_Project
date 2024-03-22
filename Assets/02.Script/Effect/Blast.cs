using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blast : MonoBehaviour
{
    public float power;
    public float upperPower;
    public float range;

    public float damage;

    private Collider[] colliders;

    public LayerMask layermask;
    
    public GameObject blaseParticle;

    private Rigidbody _rigidbody;
    private SphereCollider _collider;

    void Awake()
    {
        //layermask = ??
        colliders = Physics.OverlapSphere(transform.position, range);
    }
    
    void Start()
    {
        GameObject particle = Instantiate(blaseParticle, transform.position, Quaternion.identity);
        BlastRigidbody();
        Destroy(particle, 3f);
    }

    void BlastRigidbody()
    {
        _rigidbody.AddExplosionForce(power, transform.position, range, upperPower);
        //Damage() -> Need Check Process that player's location is inside the blast range
    }

    void Damage(PlayerProperty player)
    {
        player.TakeDamage(damage); 
    }
}
