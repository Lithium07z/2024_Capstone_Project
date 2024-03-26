using UnityEngine;
using Photon.Pun;

// 씬 이동 관련 내용을 담는 스크립트
// 메인 화면 <-> 게임에 필요한 기능 위주로 구현
public class SceneLoader : MonoBehaviour
{
    private static SceneLoader instance = null;
    
    private GameObject[] spawnPoints;
    
    public static SceneLoader Instance
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

    public void LoadGameScene()
    {
        PhotonNetwork.LoadLevel("GameScene");
    }

    public void LoadMainScene()
    {
        PhotonNetwork.LoadLevel("StartScene");
    }
    
    // 씬 이동 후에 플레이어 스폰하는 함수 
    public void PlayerSpawn()
    {
        
    }
    
    
}
