using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CargoBlast : Cargo
{
    public GameObject blastParticle;
    private PhotonView photonview;

    void Start()
    {
        photonview = GetComponent<PhotonView>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            photonview.RPC("Blast", RpcTarget.All);
        }
    }
    
    [PunRPC]
    public void Blast()
    {
        base.Broke();
        Instantiate(blastParticle, transform.position, Quaternion.identity);
    }
}
