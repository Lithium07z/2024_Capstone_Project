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
    
    void Start()
    {
        _photonView = GetComponent<PhotonView>();
    }

    public void LoadButtonClick()
    {
        _photonView.RPC("CreateCargo", RpcTarget.All);
    }

    public void UnloadButtonClick()
    {
        _photonView.RPC("DestroyCargo", RpcTarget.All);
    }

    void SetAnchorHeight(float y)
    {
        var position = cargoAnchor.transform.position;
        position = new Vector3(position.x, position.y + y + cargoHeightOffset, position.z);
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
        if (cargoList.Count > 0)
        {
            int deleteElement = cargoList.Count - 1;
            GameObject deleteBox = cargoList[deleteElement];
                
            SetAnchorHeight(-1 * deleteBox.GetComponent<CargoProperty>().height);
            cargoList.Remove(cargoList[deleteElement]);
            
            Destroy(deleteBox);
        }
        else
        {
            Debug.Log("없앨 박스가 없음!");
        }
    }
}