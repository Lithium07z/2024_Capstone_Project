using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;

public class CargoStack : MonoBehaviour
{
    public GameObject cargo;
    public GameObject cargoAnchor;


    private List<GameObject> cargoList = new List<GameObject>();
    private float cargoHeightOffset = 0.02f;
    private PhotonView _photonView;
    private ButtonManager _buttonManager;

    void Start()
    {
        _photonView = GetComponent<PhotonView>();
        _buttonManager = GameObject.Find("ButtonManager").GetComponent<ButtonManager>();
        _buttonManager.loadButton.onClick.AddListener(LoadButtonClick);
        _buttonManager.unloadButton.onClick.AddListener(UnloadButtonClick);
    }

    public void LoadButtonClick()
    {
        if (_photonView.IsMine)
            _photonView.RPC("CreateCargo", RpcTarget.AllBuffered);
    }

    public void UnloadButtonClick()
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
        newBox.transform.SetParent(this.transform);

        SetAnchorHeight(newBox.GetComponent<CargoProperty>().height);
        cargoList.Add(newBox);
    }

    [PunRPC]
    void DestroyCargo()
    {
        Debug.Log("버튼 눌림");
        int deleteElement = cargoList.Count - 1;
        GameObject deleteBox = cargoList[deleteElement];

        SetAnchorHeight(-1 * deleteBox.GetComponent<CargoProperty>().height);
        cargoList.Remove(cargoList[deleteElement]);

        Destroy(deleteBox);
    }
}