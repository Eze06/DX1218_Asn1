using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Rendering;

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

    private void Start()
    {
        this.playerController = GetComponent<PlayerController>();
        CurrentGun = GunList[0];
    }

    private void Update()
    {
        HandleSooting();
    }


    public void HandleSooting()
    {
        if (CurrentGun == null)
            return;

        if(CurrentGun.gunData.fireMode == GunData.FireMode.PRIMARY_FIRE)
        {
            switch(CurrentGun.gunData.primaryShootMode)
            {
                case GunData.ShootMode.AUTO:

                    if(playerController.shootAction.IsPressed())
                    {
                        CurrentGun.Shoot(FPSCamera);
                    }
                    break;

                case GunData.ShootMode.BURST:
                    break;
                case GunData.ShootMode.SINGLE:
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
                    break;
                case GunData.ShootMode.BURST:
                    break;
                case GunData.ShootMode.SINGLE:
                    break;
                case GunData.ShootMode.SHOTGUN:
                    break;
            }
        }


    }




    public void SelectPrimaryGun()
    {
        if (PrimaryGun == null) return;

        CurrentGun = PrimaryGun;
    }

    public void SelectSecondaryGun()
    {
        if (SecondaryGun == null) return;

        CurrentGun = SecondaryGun;
    }
}
