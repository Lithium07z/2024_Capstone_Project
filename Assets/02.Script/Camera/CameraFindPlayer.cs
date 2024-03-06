using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class CameraFindPlayer : MonoBehaviour
{
    private CinemachineVirtualCamera _cinemachineVirtualCamera;
    private PhotonView _photonView;
    private GameObject[] playerGameObjects;

    private void Awake()
    {
        _cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    public void SettingCamera()
    {
        _photonView = PhotonView.Find(FindMyPhotonViewID());

        _cinemachineVirtualCamera.Follow = _photonView.gameObject.transform.Find("PlayerCameraRoot");
        _cinemachineVirtualCamera.LookAt = _photonView.gameObject.transform.Find("PlayerCameraRoot");
    }

    private int FindMyPhotonViewID()
    {
        playerGameObjects = GameObject.FindGameObjectsWithTag("Player");
        
        foreach (GameObject player in playerGameObjects)
        {
            PhotonView photonView = player.GetComponent<PhotonView>();

            if (photonView != null && photonView.IsMine)
            {
                return photonView.ViewID;
            }
        }

        return -1;
    }
}