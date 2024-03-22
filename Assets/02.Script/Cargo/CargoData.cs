using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Cargo Data", menuName = "Scriptable Object/Cargo Data", order = int.MaxValue)]
public class CargoData : ScriptableObject
{
    public int cargoID; // cargo Data의 ID, 내부 데이터
    public string cargoName; // cargo Data의 이름, 플레이어에게 보여짐
    public int value;// cargo의 가치. 배달 완료 시 예상 수익에 추가 됨
    public float weight;
    public float cargoHeight;// cargo의 높이. cargo 적재 알고리즘에서 로직 상 필요.

    public enum Fragile
    {
        Pizza, // 세로로 배달하면 안됨. 맛있음.
        Handle, // 조심해서 다뤄야 함. 내구도가 낮음.
        Water, // 습기에 닿으면 안됨. 물이나 증기에 닿을 시 빠르게 내구도 감소
        Ice, // 온도가 낮아지면 안됨. 왜??
        Fire, // 온도가 높아지면 안됨. 왜??2
        Bomb, // 화기에 가까이 하면 안됨. 터지거나 불 붙을 위험 있음. 놓고 총으로 쏘거나 던지면 폭발함.
        Toxic, // 일정 시간 이상 지니고 있으면 주변에 데미지를 입힘.
        Breakable, // 던지거나 충격을 받는 즉시 깨짐. 
        Decay, // 빨리 배달하지 않으면 썩어버림.
        Animal, // 놔두면 자기 혼자 조금씩 움직이거나 튀어오름.
    }

    public Fragile currentProperty { get; private set; }
}
