using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    

    void Awake()
    {
        Debug.Log("Connecting...");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("Connected to Server Sucessfully");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        PhotonNetwork.JoinOrCreateRoom("Room", null, null);
        
        Debug.Log("You Entered the Room Name: " + "Room");
        Debug.Log("Character will be Create in 5s...");
            
        Invoke("InstantiatePlayer", 5f);
        
    }

    void InstantiatePlayer()
    {
        PhotonNetwork.Instantiate("PlayerArmature", Vector3.zero, Quaternion.identity);
    }
}

/*
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public Text StatusText;
    public InputField NickNameInput;
    public string gameVersion = "1.0";


    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        PhotonNetwork.GameVersion = this.gameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.Instantiate("PlayerArmature", Vector3.zero, Quaternion.identity);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        this.CreateRoom();
    }

    void CreateRoom()
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 4 });
    }
}
*/