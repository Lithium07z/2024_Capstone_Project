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
    private float currentHP;
    private float currentStamina;
    private float weight;

    // Check Player's State is Dead or Alive
    public bool isDead { get; private set; } = false;

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
        currentHP = Math.Max(currentHP + amount, maxHP);
    }
}
