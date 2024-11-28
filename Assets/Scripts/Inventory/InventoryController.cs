using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.EventSystems;
using Unity.PlasticSCM.Editor.WebApi;

[Serializable]
public class InventoryAmmo
{
    public AmmoData ammoType;
    public int Amount;
    public int MaxAmount;
}

[DisallowMultipleComponent]
public class InventoryController : MonoBehaviour
{

    public List<Gun> GunList = new List<Gun>();

    public List<InventoryAmmo> AmmoList = new List<InventoryAmmo>();

    private PlayerController playerController;

    [SerializeField] private Camera FPSCamera;

    [HideInInspector] public Gun CurrentGun;

    private Gun PrimaryGun;
    private Gun SecondaryGun;

    [Header("Pickup Variable")]
    [SerializeField] private float PickupRange = 3f;

    private void Start()
    {
        this.playerController = GetComponent<PlayerController>();

        if(GunList == null)
        {
            CurrentGun = null;
            PrimaryGun = null;
            SecondaryGun = null;
        }
        else
        {
            CurrentGun = GunList[0];
            if (CurrentGun.gunData.weaponType == GunData.WeaponType.PRIMARY)
            {
                PrimaryGun = CurrentGun;
            }
            else
            {
                SecondaryGun = CurrentGun;
            }
        }

    }

    private void Update()
    {
        HandlePickUp();

        if (CurrentGun == null)
            return;


        HandleSooting();
        HandleFireModeSwitch();
        HandleADS();
        HandleGunSprint();

        CurrentGun.gunAnimator.Sway(playerController.mouseDelta);
        CurrentGun.gunAnimator.SwayRotation(playerController.mouseDelta);

        HandleDrop();

    }

    private void HandleDrop()
    {
        if (CurrentGun == null)
            return;

        if(playerController.DropAction.WasPressedThisFrame())
        {
            CurrentGun.Drop(FPSCamera.transform);
            if(CurrentGun == PrimaryGun)
            {
                PrimaryGun.gameObject.SetActive(false);
                PrimaryGun = null;
                if(SecondaryGun != null)
                {
                    SecondaryGun.gameObject.SetActive(true);

                    GunList.Remove(CurrentGun);
                    CurrentGun = SecondaryGun;
                }
                else
                    CurrentGun = null;

            }
            else
            {
                SecondaryGun.gameObject.SetActive(false);
                SecondaryGun = null;
                if(PrimaryGun != null)
                {
                    PrimaryGun.gameObject.SetActive(true);

                    GunList.Remove(CurrentGun);
                    CurrentGun = PrimaryGun;
                }
                else
                    CurrentGun = null;         
            }

        }
    }

    public void QuickSwitch()
    {

    }

    private void HandlePickUp()
    {

        
        if(Physics.Raycast(FPSCamera.transform.position, FPSCamera.transform.forward, out RaycastHit hitinfo, PickupRange,LayerMask.GetMask("DroppedWeapon")))
        {
            if(hitinfo.collider.GetComponent<Pickup>())
            {
                Debug.Log("Can Pick Up");
            }
        }

    }

    private void HandleGunSprint()
    {
        if (playerController.isSprinting && playerController.moveDir.magnitude >= 0f)
        {
            CurrentGun.gunAnimator.doSprint = true;
            if(playerController.characterController.isGrounded)
            {
                CurrentGun.gunBob(playerController.currentSpeedMultiplier);
            }
        }
        else
        {
            CurrentGun.gunAnimator.doSprint = false;
            
        }
    }

    private void HandleADS()
    {
        if (CurrentGun == null || playerController.isSprinting)
            return;

        if(playerController.ADSAction.IsPressed())
        {
            CurrentGun.gunAnimator.doKickBack = false;
            CurrentGun.cameraAnimation.zoomADS = true;
            CurrentGun.ADS();
        }
        else
        {
            CurrentGun.gunAnimator.doKickBack = true;
            CurrentGun.gunAnimator.DoADS = false;
            CurrentGun.cameraAnimation.zoomADS = false;

        }
    }

    private void HandleFireModeSwitch()
    {
        if (CurrentGun == null)
            return;

        if(playerController.switchFireModeAction.WasPressedThisFrame())
        {
            CurrentGun.SwitchFireMode();
            Debug.Log("switch fire mode");
        }
    }

    private void HandleSooting()
    {
        if (CurrentGun == null || playerController.isSprinting)
            return;

        if(CurrentGun.gunData.fireMode == GunData.FireMode.PRIMARY_FIRE)
        {
            switch(CurrentGun.gunData.primaryShootMode)
            {
                case GunData.ShootMode.AUTO:

                    if(playerController.shootAction.IsPressed())
                    {
                        playerController.isSprinting = false;

                        CurrentGun.Shoot(FPSCamera);
                        CurrentGun.gunAnimator.doWeaponSway = false;
                    }
                    else
                    {
                        CurrentGun.gunAnimator.doWeaponSway = true;

                    }
                    break;

                case GunData.ShootMode.BURST:

                    if (playerController.shootAction.IsPressed())
                    {

                        StartCoroutine(CurrentGun.Burst(FPSCamera));
                        CurrentGun.gunAnimator.doWeaponSway = false;
                    }
                    else
                    {
                        CurrentGun.gunAnimator.doWeaponSway = true;

                    }
                    break;

                case GunData.ShootMode.SINGLE:

                    if (playerController.shootAction.WasPressedThisFrame())
                    {
                        playerController.isSprinting = false;

                        CurrentGun.Shoot(FPSCamera);
                        CurrentGun.gunAnimator.doWeaponSway = false;
                    }
                    else
                    {
                        CurrentGun.gunAnimator.doWeaponSway = true;

                    }
                    break;
                case GunData.ShootMode.SHOTGUN:
                    break;
            }    
        }
        else
        {
            switch (CurrentGun.gunData.secondaryShootMode)
            {
                case GunData.ShootMode.AUTO:
                    if (playerController.shootAction.IsPressed())
                    {
                        CurrentGun.Shoot(FPSCamera);
                        CurrentGun.gunAnimator.doWeaponSway = false;
                    }
                    else
                    {
                        CurrentGun.gunAnimator.doWeaponSway = true;

                    }

                    break;
                case GunData.ShootMode.BURST:
                    if (playerController.shootAction.IsPressed())
                    {
                        StartCoroutine(CurrentGun.Burst(FPSCamera));
                        CurrentGun.gunAnimator.doWeaponSway = false;
                    }
                    else
                    {
                        CurrentGun.gunAnimator.doWeaponSway = true;

                    }
                    break;
                case GunData.ShootMode.SINGLE:

                    if (playerController.shootAction.WasPressedThisFrame())
                    {
                        playerController.isSprinting = false;

                        CurrentGun.Shoot(FPSCamera);
                        CurrentGun.gunAnimator.doWeaponSway = false;
                    }
                    else
                    {
                        CurrentGun.gunAnimator.doWeaponSway = true;

                    }
                    break;
                case GunData.ShootMode.SHOTGUN:
                    break;
            }
        }
    }


    private void SelectPrimaryGun()
    {
        if (PrimaryGun == null || CurrentGun == PrimaryGun) return;


        SecondaryGun.gameObject.SetActive(false);
        PrimaryGun.gameObject.SetActive(true);
        CurrentGun = PrimaryGun;
        


    }

    private void SelectSecondaryGun()
    {
        if (SecondaryGun == null || CurrentGun == SecondaryGun) return;

        PrimaryGun.gameObject.SetActive(true);
        SecondaryGun.gameObject.SetActive(true);
        CurrentGun = SecondaryGun;
    }
}
