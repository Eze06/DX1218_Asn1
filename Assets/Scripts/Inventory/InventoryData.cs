using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "InventoryData")]

[Serializable]
public class InventoryAmmo
{
    public AmmoData ammoType;
    public int Amount;
    public int MaxAmount;
}

public class InventoryData : ScriptableObject
{
    public List<Gun> GunList = new List<Gun>();

    public List<InventoryAmmo> AmmoList = new List<InventoryAmmo>();

    private void OnEnable()
    {
        GunList.Clear();
        AmmoList.Clear();

    }
}
