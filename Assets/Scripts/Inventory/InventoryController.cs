using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class InventoryController : MonoBehaviour
{
    [SerializeField] private InventoryData inventoryData;
    [SerializeField] private Camera FPSCamera;

    [HideInInspector] public Gun CurrentGun;

    private Gun PrimaryGun;
    private Gun SecondaryGun;

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
