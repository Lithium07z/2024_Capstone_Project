using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

// 씬 이동 관련 내용을 담는 스크립트
// 메인 화면 <-> 게임에 필요한 기능 위주로 구현
public class SceneLoader : MonoBehaviour
{
    private static SceneLoader instance = null;
    
    public Transform[] spawnPoints;

    private Scene nowScene;

    public bool isPlayerSpawned, isSpawnPointsArrayShuffled;
    
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
        nowScene = SceneManager.GetActiveScene();

        if (nowScene.name == "GameScene")
        {
            if (!isPlayerSpawned)
            {
                // "Spawn Points" 오브젝트를 찾아옴
                GameObject spawnPointsObject = GameObject.Find("Spawn Points");

                // "Spawn Points" 오브젝트가 존재한다면
                if (spawnPointsObject != null)
                {
                    // "Spawn Points" 오브젝트의 자식들을 모두 가져와서 배열에 넣음
                    spawnPoints = spawnPointsObject.GetComponentsInChildren<Transform>();

                    // 배열의 첫 번째 요소는 부모 자신이므로 제거
                    spawnPoints = RemoveParentFromList(spawnPoints);
                }
                else
                {
                    Debug.LogError("Spawn Points object not found!");
                }
                
                ShuffleSpawnPointsArray(spawnPoints);
                PlayerSpawn();
                isPlayerSpawned = true;
            }
        }
    }
    
    // 부모 오브젝트를 제외한 자식 오브젝트들만으로 이루어진 배열을 반환하는 함수
    private Transform[] RemoveParentFromList(Transform[] _list)
    {
        // 배열 크기를 부모를 제외한 자식의 개수로 조정
        var newList = new Transform[_list.Length - 1];

        // 첫 번째 요소는 부모 오브젝트이므로 제외하고 나머지를 새 배열에 복사
        for (int i = 1; i < _list.Length; i++)
        {
            newList[i - 1] = _list[i];
        }

        return newList;
    }
    
    // 배열을 랜덤하게 재배열하는 함수
    private void ShuffleSpawnPointsArray(Transform[] _array)
    {
        var rnd = new System.Random();
        var n = _array.Length;
        while (n > 1)
        {
            n--;
            var k = rnd.Next(n + 1);
            (_array[k], _array[n]) = (_array[n], _array[k]);
        }
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
        // Photon Network를 사용하여 각 플레이어에게 고유한 시작 위치 부여
        int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1; // Photon Network에서 플레이어의 고유 인덱스를 가져옴
        
        if (playerIndex >= 0 && playerIndex < spawnPoints.Length)
        {
            Transform selectedSpawnPoint = spawnPoints[playerIndex];
            PhotonNetwork.Instantiate("TestCapsule", selectedSpawnPoint.position, selectedSpawnPoint.rotation);
        }
    }

    public string GetSceneName()
    {
        return nowScene.name;
    }
    
}
