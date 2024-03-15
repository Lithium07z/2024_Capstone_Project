using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            GameManager.instance.SetCursorForUI(true);
        }
        if(Input.GetKeyDown(KeyCode.L))
            GameManager.instance.SetCursorForUI(false);
    }
}
