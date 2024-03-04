using System.Collections;
using System.Collections.Generic;
using Inventory.Scripts.Core.ScriptableObjects.Items;
using Photon.Pun;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private ItemDataSo itemSo;
    PhotonView photonView; 

    void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    void Update()
    {

    }

    public ItemDataSo GetItemDataSo()
    {
        return itemSo;
    }

    public void DestroyItem()
    {
        photonView.RPC("SelfDestroy", RpcTarget.AllBufferedViaServer);
    }

    [PunRPC]
    private void SelfDestroy()
    {
        Destroy(this.gameObject);
    }
}
