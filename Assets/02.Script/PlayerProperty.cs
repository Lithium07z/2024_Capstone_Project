using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerProperty : MonoBehaviour
{
    // Player's Max-value presets
    public float maxHP;
    
    public float maxStamina;
    public float staminaRegeneration;
    public float maxWeight;

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
}
