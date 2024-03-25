using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerProperty : MonoBehaviour
{
    // Player's Max-value presets
    public float maxHP = 100f;

    public float maxStamina = 100f;
    public float staminaRegeneration = 10f;
    public float maxWeight = 100f;

    // Player's propertiy fields
    [SerializeField] private float currentHP;
    [SerializeField] private float currentStamina;
    private float currentWeight;

    // Check Player's State is Dead or Alive
    public bool isDead { get; private set; } = false;

    void Start()
    {
        currentHP = maxHP;
        currentStamina = maxStamina;
        currentWeight = 0;
    }
    
    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        if (damage <= 0)
        {
            isDead = true;
        }
    }

    public void Heal(float amount)
    {
        currentHP = Math.Min(currentHP + amount, maxHP);
        
    }
    
    [PunRPC]
    void RPC_TakeDamage(float damage)
    {
        currentHP -= damage;

        if (currentHP <= 0)
        {
            // 플레이어가 죽었을때 처리
        }
    }
    
    void Die()
    {
        
    }
}
