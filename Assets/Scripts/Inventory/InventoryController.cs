using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.EventSystems;
using Unity.PlasticSCM.Editor.WebApi;
using System.Linq;

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

    [SerializeField] List<Gun> GunsEquipped = new List<Gun>();

    public List<InventoryAmmo> AmmoList = new List<InventoryAmmo>();

    private PlayerController playerController;

    [SerializeField] private Camera FPSCamera;

    public Gun CurrentGun;

     public Gun PrimaryGun;
     public Gun SecondaryGun;

    [Header("Pickup Variable")]
    [SerializeField] private float PickupRange = 3f;

    private void Awake()
    {
        this.playerController = GetComponent<PlayerController>();

        foreach(Gun gun in GunList)
        {
            gun.Init();
        }
        if(GunsEquipped.Count == 0)
        {
            CurrentGun = null;
            PrimaryGun = null;
            SecondaryGun = null;
        }
        else
        {
            CurrentGun = GunsEquipped[0];
            if (CurrentGun.gunData.weaponType == GunData.WeaponType.PRIMARY)
            {
                PrimaryGun = CurrentGun;
            }
            else
            {
                SecondaryGun = CurrentGun;
            }

            CurrentGun = GunsEquipped[1];
            if (CurrentGun.gunData.weaponType == GunData.WeaponType.PRIMARY)
            {
                PrimaryGun = CurrentGun;
            }
            else
            {
                SecondaryGun = CurrentGun;
            }

            CurrentGun.gameObject.SetActive(true);

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
        HandleReload();



        CurrentGun.gunAnimator.Sway(playerController.mouseDelta);
        CurrentGun.gunAnimator.SwayRotation(playerController.mouseDelta);

        if (playerController.SelectPrimaryAction.WasPressedThisFrame())
        {
            SelectPrimaryGun();
        }
        if (playerController.SelectSecondaryAction.WasPressedThisFrame())
        {
            SelectSecondaryGun();
        }

        HandleDrop();

    }

    private void HandleReload()
    {
        if (playerController.ReloadAction.WasPressedThisFrame())
        {
            for(int i = 0; i < AmmoList.Count;i++)
            {
                if (AmmoList[i].ammoType == CurrentGun.gunData.ammoData)
                {
                    CurrentGun.Reload(AmmoList[i]);
                    break;
                }
                continue;
            }
        }
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

                    GunsEquipped.Remove(CurrentGun);
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

                    GunsEquipped.Remove(CurrentGun);
                    CurrentGun = PrimaryGun;
                }
                else
                    CurrentGun = null;         
            }

        }
    }

    private void HandlePickUp()
    {

        if(Physics.Raycast(FPSCamera.transform.position, FPSCamera.transform.forward, out RaycastHit hitinfo, PickupRange))
        {
            if (hitinfo.collider.GetComponent<Pickup>() && playerController.InteractAction.WasPressedThisFrame())
            {
                PickUpItem(hitinfo.collider.GetComponent<Pickup>());
            }
        }
        
    }
    
    public void PickUpItem(Pickup itemToPickup)
    {
        if (itemToPickup.gunData != null)
        {
            Gun gunToPickup = null;
            foreach(Gun guns in GunList)
            {
                if(guns.gunData == itemToPickup.gunData)
                {
                    gunToPickup = guns;
                    break;
                }
            }

            switch(gunToPickup.gunData.weaponType)
            {
                case GunData.WeaponType.PRIMARY:
                    if (PrimaryGun == null)
                    {
                        PrimaryGun = gunToPickup;
                        SelectPrimaryGun();
                    }
                    else
                    {
                        PrimaryGun.Drop(FPSCamera.transform);
                    }
                    break;

                case GunData.WeaponType.SECONDARY:
                    if (SecondaryGun == null)
                    {
                        SecondaryGun = gunToPickup;
                        SelectSecondaryGun();
                    }
                    else
                    {
                        SecondaryGun.Drop(FPSCamera.transform);
                    }
                    break;
            }

            Destroy(itemToPickup.gameObject);

            if (gunToPickup != null)
                GunsEquipped.Add(gunToPickup);
        }

        else if (itemToPickup.ammoData != null)
        {

        }
        else
            return;
    }

    private void HandleGunSprint()
    {
        if (playerController.isSprinting && playerController.moveDir.magnitude >= 0f)
        {
            CurrentGun.gunAnimator.doSprint = true;
            if(playerController.characterController.isGrounded)
            {
                CurrentGun.GunBob(playerController.currentSpeedMultiplier);
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
                        Debug.Log(CurrentGun.gunAnimator);
                        CurrentGun.gunAnimator.doWeaponSway = true;

                    }
                    break;
                case GunData.ShootMode.SHOTGUN:

                    if (playerController.shootAction.WasPressedThisFrame())
                    {
                        playerController.isSprinting = false;

                        CurrentGun.ShotGunShot(FPSCamera);
                        CurrentGun.gunAnimator.doWeaponSway = false;
                    }
                    else
                    {
                        CurrentGun.gunAnimator.doWeaponSway = true;

                    }

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

        if(SecondaryGun != null)
        {
            SecondaryGun.gameObject.SetActive(false);
        }

        CurrentGun = PrimaryGun;
        PrimaryGun.gameObject.SetActive(true);
        


    }

    private void SelectSecondaryGun()
    {
        if (SecondaryGun == null || CurrentGun == SecondaryGun) return;

        if(PrimaryGun != null)
        {
            PrimaryGun.gameObject.SetActive(false);
        }
        CurrentGun = SecondaryGun;

        SecondaryGun.gameObject.SetActive(true);
    }
}
