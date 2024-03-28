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
    // [SerializeField]
    public float currentHP;
    [SerializeField] private float currentStamina;
    private float currentWeight;

    private PhotonView PV;

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
        Debug.Log(currentHP);
        // 테스트 후에 수정
        // PV.RPC(nameof(RPC_TakeDamage), RpcTarget.AllBufferedViaServer, damage);
        PV.RPC(nameof(RPC_TakeDamage), PV.Owner, damage);
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
            PV.RPC("RPC_Dead", RpcTarget.AllBufferedViaServer);
        }
    }

    [PunRPC]
    void RPC_Dead()
    {
        isDead = true;
    }
}
