using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("LoginPanel")]
    public GameObject loginPanel;
    public TMP_InputField nickNameInput;

    [Header("LobbyPanel")]
    public GameObject lobbyPanel;
    // public TMP_InputField RoomInput;
    public TMP_Text WelcomeText;
    public TMP_Text lobbyInfoText;
    // public Button[] CellBtn;
    // public Button PreviousBtn;
    // public Button NextBtn;
    
    [Header("RoomPanel")]
    public GameObject roomPanel;
    // public TMP_Text ListText;
    public TMP_Text roomInfoText;
    public TMP_Text timerText;
    private bool isJoinedRoom;
    private float matchingTime;
    private int min, sec;
    // public TMP_Text[] ChatText;
    // public TMP_InputField ChatInput;
    // public Button StartGameBtn;

    [Header("ETC")]
    public TMP_Text statusText;
    public PhotonView PV;

    // List<RoomInfo> myList = new List<RoomInfo>();
    // int currentPage = 1, maxPage, multiple;


    // #region 방리스트 갱신
    // // ◀버튼 -2 , ▶버튼 -1 , 셀 숫자
    // public void MyListClick(int num)
    // {
    //     if (num == -2) --currentPage;
    //     else if (num == -1) ++currentPage;
    //     else PhotonNetwork.JoinRoom(myList[multiple + num].Name);
    //     MyListRenewal();
    // }
    //
    // void MyListRenewal()
    // {
    //     // 최대페이지
    //     maxPage = (myList.Count % CellBtn.Length == 0) ? myList.Count / CellBtn.Length : myList.Count / CellBtn.Length + 1;
    //
    //     // 이전, 다음버튼
    //     PreviousBtn.interactable = (currentPage <= 1) ? false : true;
    //     NextBtn.interactable = (currentPage >= maxPage) ? false : true;
    //
    //     // 페이지에 맞는 리스트 대입
    //     multiple = (currentPage - 1) * CellBtn.Length;
    //     for (int i = 0; i < CellBtn.Length; i++)
    //     {
    //         CellBtn[i].interactable = (multiple + i < myList.Count) ? true : false;
    //         CellBtn[i].transform.GetChild(0).GetComponent<TMP_Text>().text = (multiple + i < myList.Count) ? myList[multiple + i].Name : "";
    //         CellBtn[i].transform.GetChild(1).GetComponent<TMP_Text>().text = (multiple + i < myList.Count) ? myList[multiple + i].PlayerCount + "/" + myList[multiple + i].MaxPlayers : "";
    //     }
    // }
    //
    // public override void OnRoomListUpdate(List<RoomInfo> roomList)
    // {
    //     int roomCount = roomList.Count;
    //     for (int i = 0; i < roomCount; i++)
    //     {
    //         if (!roomList[i].RemovedFromList)
    //         {
    //             if (!myList.Contains(roomList[i])) myList.Add(roomList[i]);
    //             else myList[myList.IndexOf(roomList[i])] = roomList[i];
    //         }
    //         else if (myList.IndexOf(roomList[i]) != -1) myList.RemoveAt(myList.IndexOf(roomList[i]));
    //     }
    //     MyListRenewal();
    // }
    // #endregion


    #region 서버연결
    void Awake() => Screen.SetResolution(960, 540, false);

    void Update()
    {
        statusText.text = PhotonNetwork.NetworkClientState.ToString();
        lobbyInfoText.text = (PhotonNetwork.CountOfPlayers - PhotonNetwork.CountOfPlayersInRooms) + " in Lobby / " + PhotonNetwork.CountOfPlayers + " Connected";

        if (isJoinedRoom && matchingTime > 0)
        {
            matchingTime -= Time.deltaTime;
            // 남은 시간을 분과 초로 변환
            min = Mathf.FloorToInt(matchingTime / 60f);
            sec = Mathf.FloorToInt(matchingTime % 60f);

            // UI에 표시
            timerText.text = string.Format("{0:00}:{1:00}", min, sec);
        }

        if (isJoinedRoom && matchingTime <= 0)
        {
            PhotonNetwork.LoadLevel("GameScene");
            isJoinedRoom = false;
        }
    }

    public void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster()
    {   
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        loginPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        roomPanel.SetActive(false);
        PhotonNetwork.LocalPlayer.NickName = nickNameInput.text;
        WelcomeText.text = "Welcome! " + PhotonNetwork.LocalPlayer.NickName;
        // myList.Clear();
    }

    public void Disconnect() => PhotonNetwork.Disconnect();

    public override void OnDisconnected(DisconnectCause cause)
    {
        loginPanel.SetActive(true);
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(false);
    }
    #endregion


    #region 방
    public void CreateRoom() => PhotonNetwork.CreateRoom("Room" + Random.Range(0, 100), new RoomOptions { MaxPlayers = 4 });

    public void JoinRandomRoom() => PhotonNetwork.JoinRandomRoom();

    public void LeaveRoom()
    {
        lobbyPanel.SetActive(true);
        isJoinedRoom = false;
        PhotonNetwork.LeaveRoom();
    }

    public override void OnJoinedRoom()
    {
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);
        RoomRenewal();
        isJoinedRoom = true;
        PV.RPC("ResetMatchingTimeRPC", RpcTarget.All);
        // ChatInput.text = "";
        // for (int i = 0; i < ChatText.Length; i++) ChatText[i].text = "";

        // 방장이 아니면 게임시작 버튼을 누를 수 없음.
        // StartGameBtn.interactable = PhotonNetwork.IsMasterClient;
    }

    public override void OnCreateRoomFailed(short returnCode, string message) { CreateRoom(); } 

    public override void OnJoinRandomFailed(short returnCode, string message) { CreateRoom(); }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        RoomRenewal();
        // ChatRPC("<color=yellow>" + newPlayer.NickName + "님이 참가하셨습니다</color>");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RoomRenewal();
        // ChatRPC("<color=yellow>" + otherPlayer.NickName + "님이 퇴장하셨습니다</color>");
    }

    void RoomRenewal()
    {
        // ListText.text = "";
        // for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        //     ListText.text += PhotonNetwork.PlayerList[i].NickName + ((i + 1 == PhotonNetwork.PlayerList.Length) ? "" : ", ");
        roomInfoText.text = PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;
    }

    [PunRPC]
    void ResetMatchingTimeRPC()
    {
        matchingTime = 10.0f;
    }
    
    // 게임 시작 버튼을 누르면 호출되는 함수
    // public void StartGame()
    // {   
    //     PhotonNetwork.LoadLevel(1);
    // }
    //
    // public override void OnMasterClientSwitched(Player newMasterClient)
    // {
    //     StartGameBtn.interactable = PhotonNetwork.IsMasterClient;
    // }
    #endregion


    // #region 채팅
    // public void Send()
    // {
    //     PV.RPC("ChatRPC", RpcTarget.All, PhotonNetwork.NickName + " : " + ChatInput.text);
    //     ChatInput.text = "";
    // }
    //
    // [PunRPC] // RPC는 플레이어가 속해있는 방 모든 인원에게 전달한다
    // void ChatRPC(string msg)
    // {
    //     bool isInput = false;
    //     for (int i = 0; i < ChatText.Length; i++)
    //     {
    //         if (ChatText[i].text == "")
    //         {
    //             isInput = true;
    //             ChatText[i].text = msg;
    //             break;
    //         }
    //     }
    //     if (!isInput) // 꽉차면 한칸씩 위로 올림
    //     {
    //         for (int i = 1; i < ChatText.Length; i++) ChatText[i - 1].text = ChatText[i].text;
    //         ChatText[ChatText.Length - 1].text = msg;
    //     }
    // }
    // #endregion
}
