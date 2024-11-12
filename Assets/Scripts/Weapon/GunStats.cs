using UnityEngine;

[CreateAssetMenu(menuName="Weapon/GunStats")]
public class GunStats : ScriptableObject
{
    [SerializeField]
    private LayerMask HitMask;
    [SerializeField]
    private Vector3 Spread = new Vector3(0.1f, 0.1f, 0.1f);
    [SerializeField]
    private float FireRate = 0.25f;
}
