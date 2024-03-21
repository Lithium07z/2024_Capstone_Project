using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
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
            
    }
}
