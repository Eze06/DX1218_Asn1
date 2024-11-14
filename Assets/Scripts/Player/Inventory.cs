using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory")]
public class Inventory : ScriptableObject
{
    public List<GunSO> GunInv = new List<GunSO>();

    private void OnEnable()
    {
        GunInv.Clear();
    }
}
