using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoCreator : MonoBehaviour
{
    public Transform CreatePosition;
    
    public void CreateCargo(GameObject cargo)
    {
        Instantiate(cargo, CreatePosition.position, CreatePosition.rotation);
    }
}
