using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class CameraFindPlayer : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cvc;

    private PhotonView _photonView;

    private GameObject[] playerGameObjects;

    public void SettingCamera()
    {
        _photonView = PhotonView.Find(FindMyPhotonViewID());

        cvc.Follow = _photonView.gameObject.transform.Find("PlayerCameraRoot");
        cvc.LookAt = _photonView.gameObject.transform.Find("PlayerCameraRoot");
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