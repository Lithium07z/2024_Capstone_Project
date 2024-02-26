using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Serialization;

public class PlayerIconController : MonoBehaviour
{
    public GameObject playerIcon;
    private Renderer _renderer;

    private PhotonView _photonView;
    // Start is called before the first frame update
    void Start()
    {
        _renderer = playerIcon.GetComponent<Renderer>();
        _photonView = GetComponent<PhotonView>();

        if (_photonView.IsMine)
        {
            _renderer.material.color = Color.red;
        }
    }
}
