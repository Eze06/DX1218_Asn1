using UnityEngine;

[CreateAssetMenu(menuName="Weapon/GunStats")]
public class GunStats : ScriptableObject
{
    public LayerMask HitMask;
    public Vector3 Spread = new Vector3(0.1f, 0.1f, 0.1f);
    public float FireRate = 0.25f;

    [Header("Ammo Variables")]
    [HideInInspector] public int CurrentAmmo;
    [HideInInspector] public int CurrentMags;

    public int MagazineSize;
    public int MaxMagazines;

    
}