using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public CargoStack _cargostack;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            UIManager.Instance.SetCursorForUI(true);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            UIManager.Instance.SetCursorForUI(false);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _cargostack.LoadCargo();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _cargostack.UnloadCargo();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _cargostack.UnloadCargo(3);
        }
    }
}
