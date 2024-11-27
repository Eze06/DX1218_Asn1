using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName="Weapon/GunData")]
public class GunData : ScriptableObject
{

    public enum ReloadMode
    {
        MAGAZINES,
        SINGLEBULLET
    }

    public enum ShootMode
    {   
        AUTO,
        BURST,
        SINGLE,
        SHOTGUN
    }

    public enum ShootType
    {
        HITSCAN,
        PROJECTILE
    }

    public enum WeaponType
    {
        PRIMARY,
        SECONDARY
    }    

    public enum FireMode
    {
        PRIMARY_FIRE,
        SECONDARY_FIRE,
    }

    public LayerMask HitMask;

    [Header("Gun Variables")]
    public string gunName = " ";
    public WeaponType weaponType;
    public GameObject droppablePrefab;

    [Header("Shooting Variables")]
    public float FireRate = 0.25f;
    public ShootMode primaryShootMode;
    public ShootMode secondaryShootMode;

    public ShootType weaponShootType;
    [HideInInspector] public FireMode fireMode;

    [Header("Recoil Variables")]
    public Vector3 Spread = new Vector3(0.1f, 0.1f, 0.1f);
    public float returnSpeed;
    public float snappiness;
    public float kickBackAmt = 5f;


    [Header("Burst Fire")]
    public int BulletsPerBurst;
    [Range(0,0.2f)]
    public float TimeBetweenBurst;

    [Header("Magazine Variables")]
    public AmmoData ammoData;
    public int RoundsPerMag = 30;


    private void OnEnable()
    {
        fireMode = FireMode.PRIMARY_FIRE;
    }

}
