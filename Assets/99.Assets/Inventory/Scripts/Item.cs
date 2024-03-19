using Inventory.Scripts.Core.ScriptableObjects.Items;
using Photon.Pun;
using UnityEngine;

enum itemTag { Pistol, Backpack };

public class Item : MonoBehaviour
{

    [SerializeField] private ItemDataSo _itemSo;
    PhotonView _photonView;

    //itemTag _itemTag = itemTag.Pistol;

    void Start()
    {
        _photonView = GetComponent<PhotonView>();
    }

    void Update()
    {

    }

    public ItemDataSo GetItemDataSo()
    {
        return _itemSo;
    }

    public void DestroyItem()
    {
        _photonView.RPC("SelfDestroy", RpcTarget.AllBufferedViaServer);
    }

    [PunRPC]
    private void SelfDestroy()
    {
        Destroy(this.gameObject);
    }
}
