using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

// MainScene에서 Photon Network가 동작하는 스크립트
// PhotonNetwork. 어쩌구 코드들만 남아있을 것
public class NetworkManager : MonoBehaviourPunCallbacks
{
    private static NetworkManager instance = null;
    public PhotonView PV;
    
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

        if (isJoinedRoom && matchingTime <= 0)
        {
            SceneLoader.Instance.LoadGameScene();
            isJoinedRoom = false;
        }

        CalcMatchingTime();
    }

    private void CalcMatchingTime()
    {
        if (isJoinedRoom && matchingTime > 0)
        {
            matchingTime -= Time.deltaTime;
            // 남은 시간을 분과 초로 변환
            min = Mathf.FloorToInt(matchingTime / 60f);
            sec = Mathf.FloorToInt(matchingTime % 60f);

            // UI에 표시
            MainGUIManager.Instance.UpdateTimerText(min, sec);
        }
    }
    
    #region 포톤서버연결, 로비연결
    public void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster()
    {   
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        PhotonNetwork.LocalPlayer.NickName = MainGUIManager.Instance.GetUserName();
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
    }

    public override void OnCreateRoomFailed(short returnCode, string message) { CreateRoom(); } 

    public override void OnJoinRandomFailed(short returnCode, string message) { CreateRoom(); }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        MainGUIManager.Instance.RoomRenewal(PhotonNetwork.CurrentRoom.PlayerCount, PhotonNetwork.CurrentRoom.MaxPlayers);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        MainGUIManager.Instance.RoomRenewal(PhotonNetwork.CurrentRoom.PlayerCount, PhotonNetwork.CurrentRoom.MaxPlayers);
    }

    [PunRPC]
    private void ResetMatchingTimeRPC()
    {
        matchingTime = 10.0f;
    }
    
    #endregion
    
}
