using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{

    [SerializeField] InventoryController inventory;

    [SerializeField] SlotUI PrimarySlot;
    [SerializeField] SlotUI SecondarySlot;
    [SerializeField] TMP_Text AmmoCount;

    // Update is called once per frame
    void Update()
    {
        HandleSlotUI();
        HandleAmmoCount();
    }

    private void HandleAmmoCount()
    {
        if (inventory.CurrentGun == null)
            return;

        string TotalAmmoCount = "0";
        
        foreach(InventoryAmmo invAmmo in inventory.AmmoList)
        {
            if(invAmmo.ammoType == inventory.CurrentGun.gunData.ammoData)
            {
                TotalAmmoCount = invAmmo.Amount.ToString();
                break;
            }
        }

        string AmmoText = inventory.CurrentGun.CurrentRounds.ToString() + " / " + TotalAmmoCount;
        AmmoCount.text = AmmoText;
    }

    private void HandleSlotUI()
    {
        if (inventory.PrimaryGun == null)
        {
            PrimarySlot.Display(false);
        }
        else
        {
            PrimarySlot.Display(true, inventory.PrimaryGun);
        }

        if (inventory.SecondaryGun == null)
        {
            SecondarySlot.Display(false);
        }
        else
        {
            SecondarySlot.Display(true, inventory.SecondaryGun);
        }

        if(inventory.CurrentGun == inventory.PrimaryGun)
        {
            PrimarySlot.Select();
            SecondarySlot.Deselect();
        }
        else if (inventory.CurrentGun == inventory.SecondaryGun)
        {
            SecondarySlot.Select();
            PrimarySlot.Deselect();
        }
    }
}
