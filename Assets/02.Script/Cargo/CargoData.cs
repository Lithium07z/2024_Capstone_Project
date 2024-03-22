using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Cargo Data", menuName = "Scriptable Object/Cargo Data", order = int.MaxValue)]
public class CargoData : ScriptableObject
{
    public int cargoID { get; private set; }
    public int cargoName { get; private set; }
    public int value { get; private set; }
    public float weight { get; private set; }

    public float cargoHeight { get; private set; }

    public enum Fragile
    {
        Pizza,
        Handle,
        Water,
        Ice,
        Bomb,
        Toxic
    }

    public Fragile currentProperty { get; private set; }
}
