/*
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
*/

using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    public string testRoomNumber;
    public TMP_Text testRoomNumberTxt;
    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
    }

    void Start()
    {
        testRoomNumberTxt.text = "Test Room " + testRoomNumber;
        PhotonNetwork.ConnectUsingSettings();
    }

    void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom(testRoomNumber, new RoomOptions { MaxPlayers = 4 }, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.Instantiate("PlayerArmature", Vector3.zero, Quaternion.identity);
    }
}
