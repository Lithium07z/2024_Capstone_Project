using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using StarterAssets;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class GunShooter : MonoBehaviour
{
    public enum AimState
    {
        Idle,
        Aim,
    }
    
    public AimState aimState { get; private set; }
    
    
    // Current Weapon's Property Class Ref
    private GunProperty _gunProperty;
    private GameObject currentGun;
    public GameObject gun;
    public Transform gunPivot;
    private Transform leftHandIKPivot;
    
    
    // Character's Component
    private Animator _anim;
    private StarterAssetsInputs _input;
    private Transform playerSpine;

    

    public GameObject Test;
    
    // Animator Parameters
    private static readonly int _animIDAim = Animator.StringToHash("Aim");

    void Start()
    {
        _input = GetComponent<StarterAssetsInputs>();
        _anim = GetComponent<Animator>();

        currentGun = Instantiate(gun, gunPivot.transform.position, gunPivot.transform.rotation);
        currentGun.transform.parent = gunPivot;
        _gunProperty = currentGun.GetComponent<GunProperty>();
        leftHandIKPivot = _gunProperty.leftIKPivot.transform;

        //playerSpine = _anim.GetBoneTransform(HumanBodyBones.Spine);
    }
    

    private void Update()
    {
        Aim();
        // 발사 로직 (추가 예정)
        /*
        if (_input.shoot)
        {
            Shoot();
        }
        */
    }

    private void LateUpdate()
    {
        //playerSpine.LookAt(Test.transform); 
    }

    void Aim()
    {
        // 에임 입력 감지
        if (_input.aim && aimState == AimState.Idle)
        {
            aimState = AimState.Aim;
            _anim.SetBool(_animIDAim, true);
        }
        else if (!_input.aim && aimState == AimState.Aim)
        {
            aimState = AimState.Idle;
            _anim.SetBool(_animIDAim, false);
        }
    }
    
    void Attack()
    {
        // 발사 메커니즘 구현
        // 예: 총알 발사, 사운드 재생, 애니메이션 트리거 등
    }

    

    private void OnAnimatorIK(int layerIndex)
    {
        if (aimState == AimState.Aim)
        {
            _anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            _anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);

            _anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHandIKPivot.position);
            _anim.SetIKRotation(AvatarIKGoal.LeftHand, leftHandIKPivot.rotation);
        }
        else
        {
            _anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
            _anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
        }
    }
}