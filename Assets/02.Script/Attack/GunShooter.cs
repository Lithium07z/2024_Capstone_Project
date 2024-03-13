using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using StarterAssets;
using UnityEngine.InputSystem;

public class GunShooter : MonoBehaviour
{
    public GameObject gun;
    
    private Animator _anim;
    private StarterAssetsInputs _input;
    private GunProperty _gunProperty;
    
    public Transform target; // 총구의 위치
    public Transform gunHandle; // 총 잡는 위치

    private bool isAiming = false; // 현재 에임 상태
    
    // Animator Parameters
    private static readonly int _animIDAim = Animator.StringToHash("Aim");

    void Start()
    {
        _input = GetComponent<StarterAssetsInputs>();
        _anim = GetComponent<Animator>();
    }

    private void Update()
    {
    
        _gunProperty = gun.GetComponent<GunProperty>();
        // 에임 입력 감지
        if (_input.aim && !isAiming)
        {
            isAiming = true;
            _anim.SetBool(_animIDAim, true);
        }/*
        else if (!_input.aim && isAiming)
        {
            isAiming = false;
            _anim.SetBool(_animIDAim, false);
        }*/

        // 발사 로직 (추가 예정)
        /*
        if (_input.shoot)
        {
            Shoot();
        }
        */
    }

    void Shoot()
    {
        // 발사 메커니즘 구현
        // 예: 총알 발사, 사운드 재생, 애니메이션 트리거 등
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (isAiming)
        {
            _anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            _anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
            _anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            _anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);

            _anim.SetIKPosition(AvatarIKGoal.LeftHand, _gunProperty.LeftHandTarget.position);
            _anim.SetIKRotation(AvatarIKGoal.LeftHand, _gunProperty.LeftHandTarget.rotation);
            _anim.SetIKPosition(AvatarIKGoal.RightHand, _gunProperty.RightHandTarget.position);
            _anim.SetIKRotation(AvatarIKGoal.RightHand, _gunProperty.RightHandTarget.rotation);
        }
        else
        {
            _anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
            _anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
        }
    }
}