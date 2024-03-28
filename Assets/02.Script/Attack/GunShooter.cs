using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using StarterAssets;

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
    private Transform muzzle;
    
    
    // Character's Component
    private Animator _anim;
    private StarterAssetsInputs _input;
    private Transform playerSpine;
    private PhotonView PV;

    [SerializeField] private LayerMask playerLayer;
    
    
    public GameObject Test;
    
    // Animator Parameters
    private static readonly int _animIDAim = Animator.StringToHash("Aim");

    void Start()
    {
        _input = GetComponent<StarterAssetsInputs>();
        _anim = GetComponent<Animator>();
        PV = GetComponent<PhotonView>();

        currentGun = Instantiate(gun, gunPivot.transform.position, gunPivot.transform.rotation);
        currentGun.transform.parent = gunPivot;
        _gunProperty = currentGun.GetComponent<GunProperty>();
        leftHandIKPivot = _gunProperty.leftIKPivot.transform;
        muzzle = _gunProperty.muzzle.transform;

        //playerSpine = _anim.GetBoneTransform(HumanBodyBones.Spine);
    }
    

    private void Update()
    {
        Aim();
        
        if (_input.shoot)
        {
            Shoot();
        }
        _input.shoot = false;
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
            UIManager.Instance.SetActiveCrosshair(true);
        }
        else if (!_input.aim && aimState == AimState.Aim)
        {
            aimState = AimState.Idle;
            _anim.SetBool(_animIDAim, false);
            UIManager.Instance.SetActiveCrosshair(false);
        }
    }

    void Shoot()
    {
        // 발사 메커니즘
        RaycastHit hit;
        Debug.DrawRay(muzzle.position, muzzle.forward * 10, Color.cyan);
        if (Physics.Raycast(muzzle.transform.position, muzzle.transform.forward, out hit, 10f))
        {
            // hit.collider.gameObject.GetComponent<PlayerProperty>()?.TakeDamage(20f);
            Debug.Log(_gunProperty.guninfo.damage);
            PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
        }
    }

    [PunRPC]
    void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal)
    {
        GameObject bulletImpactObj = Instantiate(_gunProperty.guninfo.bullet, hitPosition + hitNormal * 0.001f,
            Quaternion.LookRotation(hitNormal, Vector3.up) * _gunProperty.guninfo.bullet.transform.rotation);
        Destroy(bulletImpactObj, 10f);
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