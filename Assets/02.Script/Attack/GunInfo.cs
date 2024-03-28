using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TPS/New Gun")]
public class GunInfo : ScriptableObject
{
    public string gunName;
    public float damage;
    public GameObject bullet;
}
