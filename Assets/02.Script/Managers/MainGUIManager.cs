using UnityEngine;
using UnityEngine.UI;
using TMPro;

// MainScene에서 GUI와 상호작용할 때, 불리는 기능만 있는 스크립트
public class MainGUIManager : MonoBehaviour
{
    private static MainGUIManager instance = null;

    [Header("LoginPanel")]
    public GameObject loginPanel;
    public InputField nickNameInput;

    [Header("LobbyPanel")]
    public GameObject lobbyPanel;
    public TMP_Text userNameText;
    public TMP_Text lobbyInfoText;
    
    [Header("RoomPanel")]
    public GameObject roomPanel;
    public TMP_Text roomInfoText;
    public TMP_Text timerText;

    [Header("ETC")]
    public TMP_Text statusText;
    
    public static MainGUIManager Instance
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

    private void Awake()
    {
        Screen.SetResolution(960, 540, false);
        
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
    
    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    public void UpdatePhotonStatusText(string _statusText)
    {
        statusText.text = _statusText;
    }

    public void UpdateLobbyInfoText(int _lobbyPlayersCount, int _allPlayersCount)
    {
        lobbyInfoText.text =  _lobbyPlayersCount + " in Lobby / " + _allPlayersCount + " Connected";
    }

    public void RoomRenewal(int _roomPlayerCount, int _maxPlayerCount)
    {
        roomInfoText.text = _roomPlayerCount + " / " + _maxPlayerCount;
    }

    public void ConnectGame()
    {
        NetworkManager.Instance.Connect();
        userNameText.text = nickNameInput.text;
        loginPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        roomPanel.SetActive(false);
    }

    public void EnterRoom()
    {
        NetworkManager.Instance.JoinRandomRoom();
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);
    }

    public void LeaveRoom()
    {
        NetworkManager.Instance.LeaveRoom();
        lobbyPanel.SetActive(true);
        roomPanel.SetActive(false);
    }
    
    public void QuitGame()
    {
        NetworkManager.Instance.Disconnect();
        loginPanel.SetActive(true);
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(false);
    }

    public void UpdateTimerText(int _minutes, int _seconds)
    {
        timerText.text = string.Format("{0:00}:{1:00}", _minutes, _seconds);
    }

    public string GetUserName()
    {
        return nickNameInput.text;
    }
    
    
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
