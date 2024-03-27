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

    public void UnloadCargo(int i=0)
    {
        if (_photonView.IsMine)
            if (cargoList.Count > 0)
            {
                if(i==0)
                    _photonView.RPC("DestroyCargo", RpcTarget.AllBuffered);
                else
                    _photonView.RPC("DestroyCargo", RpcTarget.AllBuffered,3);
            }
            else
                Debug.Log("보관함에 박스가 없음!");
        
    }

    private void AddAnchorHeight(float y)
    {
        var position = cargoAnchor.transform.position;
        position = new Vector3(position.x, position.y + y, position.z);
        cargoAnchor.transform.position = position;
    }

    private void SetAnchorHeight(float y)
    {
        var position = cargoAnchor.transform.position;
        position = new Vector3(position.x, y, position.z);
        cargoAnchor.transform.position = position;
    }

    [PunRPC]
    private void CreateCargo()
    {
        GameObject newBox = PhotonNetwork.Instantiate("TestCargo", cargoAnchor.transform.position, Quaternion.identity);
        cargoList.Add(newBox);
        newBox.transform.SetParent(this.transform);
        AddAnchorHeight(newBox.GetComponent<Cargo>().cargoData.cargoHeight);
    }

    [PunRPC]
    private void DestroyCargo()
    {
        int deleteElement = cargoList.Count - 1;
        GameObject deleteBox = cargoList[deleteElement];

        AddAnchorHeight(-1 * deleteBox.GetComponent<Cargo>().cargoData.cargoHeight);
        
        cargoList.Remove(cargoList[deleteElement]);

        PhotonNetwork.Destroy(deleteBox);
    }

    [PunRPC]
    private void DestroyCargo(int index)
    {
        for (int i = cargoList.Count - 1; i > index; i--)
        {
            DestroyCargo();
        }
    }
}