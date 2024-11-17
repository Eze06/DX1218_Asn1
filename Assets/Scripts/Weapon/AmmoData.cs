using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Weapon/AmmoData")]
public class AmmoData : ScriptableObject
{
    public enum AmmoType
    {
        HITSCAN,
        PROJECTILE
    }

    public AmmoType ammoType;


    [Range(0, 50)]
    public float Damage = 10f;


    [Header("Projectile Varialbes")]
    public float ProjectileSpeed = 400f;
    public float ProjectileMass = 0f;

}
