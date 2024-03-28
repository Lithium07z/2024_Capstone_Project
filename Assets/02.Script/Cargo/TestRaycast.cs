using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRaycast : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Fire();
    }

    void Fire()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3( Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2, 0));

            RaycastHit rayhit;
            if (Physics.Raycast(ray, out rayhit, Mathf.Infinity))
            {
                Debug.Log("확인: " + rayhit.transform.name);
            }
        }
    }
}
