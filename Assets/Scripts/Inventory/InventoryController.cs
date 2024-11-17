using UnityEngine;

[DisallowMultipleComponent]
public class InventoryController : MonoBehaviour
{
    private PlayerController playerController;

    [SerializeField] public InventoryData inventoryData;
    [SerializeField] private Camera FPSCamera;

    [HideInInspector] public Gun CurrentGun;


    private Gun PrimaryGun;
    private Gun SecondaryGun;

    private void Start()
    {
        this.playerController = GetComponent<PlayerController>();
    }
    public void HandleSooting()
    {
        if (CurrentGun == null)
            return;

        


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
