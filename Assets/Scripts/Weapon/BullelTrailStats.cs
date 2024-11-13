using UnityEngine;

[CreateAssetMenu(menuName="Weapon/trailStats")]
public class BulletTrailStats : MonoBehaviour
{
    public Material material;
    public AnimationCurve WidthCurve;
    public float Duration = 0.5f;
    public float MinVertexDistance = 0.1f;
    public Gradient color;

    public float MissDistance = 100f;
    public float SimulationSpeed = 100f;
}
