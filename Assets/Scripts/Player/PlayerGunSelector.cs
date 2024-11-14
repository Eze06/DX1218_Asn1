using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerGunSelector : MonoBehaviour
{
    [SerializeField] Inventory Inv;

    private GunSO SelectedGun;
    // Start is called before the first frame update
    void Start()
    {
        if(Inv.GunInv != null)
        {
            SelectedGun = Inv.GunInv[0];
            SelectedGun.Create(this.transform, this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
