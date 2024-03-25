using System;
using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerManager : MonoBehaviour
{
    private PhotonView PV;
    private int kills;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // 로비 및 맵 생성 후 활성화
        // if (PV.IsMine)
        // {
        //     CreateController();
        // }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // 비 및 맵 생성 후 활성화 추가 계획
    // void CreateController()
    // {
    //     // 스폰위치 및 캐릭터 생성 코드 작성 예정
    // }

    public void GetKill()
    {
        PV.RPC(nameof(RPC_GetKill), PV.Owner);
        
    }

    public void Die()
    {
        
    }

    [PunRPC]
    void RPC_GetKill()
    {
        kills++;

        Hashtable hash = new Hashtable();
        hash.Add("Kills", kills);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }
}
