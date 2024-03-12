using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GunProperty : MonoBehaviour
{
    [FormerlySerializedAs("LeftHandIK")] public Transform LeftHandTarget;
    [FormerlySerializedAs("RightHandIK")] public Transform RightHandTarget;
}
