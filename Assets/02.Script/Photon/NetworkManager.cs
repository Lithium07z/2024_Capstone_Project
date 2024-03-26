using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

// MainScene에서 Photon Network가 동작하는 스크립트
// PhotonNetwork. 어쩌구 코드들만 남아있을 것
public class NetworkManager : MonoBehaviourPunCallbacks
{
    public PhotonView PV;
    private static NetworkManager instance = null;
    
    private bool isJoinedRoom;
    private float matchingTime;
    private int min, sec;
    
    public static NetworkManager Instance
    {
        get
        {
            if (instance is null)
            {
                return null;
            }

            return instance;
        }
    }

    #region 서버연결

    void Awake()
    {
        if (instance is null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        MainGUIManager.Instance.UpdatePhotonStatusText(PhotonNetwork.NetworkClientState.ToString());
        MainGUIManager.Instance.UpdateLobbyInfoText((PhotonNetwork.CountOfPlayers - PhotonNetwork.CountOfPlayersInRooms), PhotonNetwork.CountOfPlayers);

        if (isJoinedRoom && matchingTime > 0)
        {
            matchingTime -= Time.deltaTime;
            // 남은 시간을 분과 초로 변환
            min = Mathf.FloorToInt(matchingTime / 60f);
            sec = Mathf.FloorToInt(matchingTime % 60f);

            // UI에 표시
            MainGUIManager.Instance.UpdateTimerText(min, sec);
        }

        if (isJoinedRoom && matchingTime <= 0)
        {
            SceneLoader.Instance.LoadGameScene();
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
        PhotonNetwork.LocalPlayer.NickName = MainGUIManager.Instance.GetUserName();
        // myList.Clear();
    }

    public void Disconnect() => PhotonNetwork.Disconnect();
    
    #endregion


    #region 방
    private void CreateRoom() => PhotonNetwork.CreateRoom("Room" + Random.Range(0, 100), new RoomOptions { MaxPlayers = 4 });

    public void JoinRandomRoom() => PhotonNetwork.JoinRandomRoom();

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        isJoinedRoom = false;
    }
    
    public override void OnJoinedRoom()
    {
        MainGUIManager.Instance.RoomRenewal(PhotonNetwork.CurrentRoom.PlayerCount, PhotonNetwork.CurrentRoom.MaxPlayers);
        isJoinedRoom = true;
        PV.RPC("ResetMatchingTimeRPC", RpcTarget.All);
        // ChatInput.text = "";
        // for (int i = 0; i < ChatText.Length; i++) ChatText[i].text = "";
    }

    public override void OnCreateRoomFailed(short returnCode, string message) { CreateRoom(); } 

    public override void OnJoinRandomFailed(short returnCode, string message) { CreateRoom(); }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        MainGUIManager.Instance.RoomRenewal(PhotonNetwork.CurrentRoom.PlayerCount, PhotonNetwork.CurrentRoom.MaxPlayers);
        // ChatRPC("<color=yellow>" + newPlayer.NickName + "님이 참가하셨습니다</color>");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        MainGUIManager.Instance.RoomRenewal(PhotonNetwork.CurrentRoom.PlayerCount, PhotonNetwork.CurrentRoom.MaxPlayers);
        // ChatRPC("<color=yellow>" + otherPlayer.NickName + "님이 퇴장하셨습니다</color>");
    }

    [PunRPC]
    void ResetMatchingTimeRPC()
    {
        matchingTime = 10.0f;
    }
    
    #endregion
    
}
