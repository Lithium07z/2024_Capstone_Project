using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;

public class CargoStack : MonoBehaviour
{
    // Cargo Prefabs & Anchors
    public GameObject cargo;
    public GameObject cargoAnchor;

    // Cargo Set
    [SerializeField] private List<GameObject> cargoList;
    private float cargoHeightOffset = 0.02f;
    
    // Photon Initialize
    private PhotonView _photonView;

    void Start()
    {
        _photonView = GetComponent<PhotonView>();
    }

    public void LoadCargo()
    {
        if (_photonView.IsMine)
            _photonView.RPC("CreateCargo", RpcTarget.AllBuffered);
    }

    public void UnloadCargo()
    {
        if (_photonView.IsMine)
            if (cargoList.Count > 0)
                _photonView.RPC("DestroyCargo", RpcTarget.AllBuffered);
            else
                Debug.Log("보관함에 박스가 없음!");
            
    }

    void SetAnchorHeight(float y)
    {
        var position = cargoAnchor.transform.position;
        position = new Vector3(position.x, position.y + y, position.z);
        cargoAnchor.transform.position = position;
    }

    [PunRPC]
    void CreateCargo()
    {
        GameObject newBox = Instantiate(cargo, cargoAnchor.transform.position, Quaternion.identity);
        cargoList.Add(newBox);
        newBox.transform.SetParent(this.transform);
        SetAnchorHeight(newBox.GetComponent<Cargo>().height);
    }

    [PunRPC]
    void DestroyCargo()
    {
        int deleteElement = cargoList.Count - 1;
        GameObject deleteBox = cargoList[deleteElement];

        SetAnchorHeight(-1 * deleteBox.GetComponent<Cargo>().height);
        
        cargoList.Remove(cargoList[deleteElement]);

        Destroy(deleteBox);
    }
}