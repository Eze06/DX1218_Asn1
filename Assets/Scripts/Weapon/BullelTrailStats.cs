using UnityEngine;

[CreateAssetMenu(menuName="Weapon/trailStats")]
public class BullelTrailStats : MonoBehaviour
{
    [SerializeField] private Material material;
    [SerializeField] private AnimationCurve WidthCurve;
    [SerializeField] private float Duration = 0.5f;
    [SerializeField] private float MinVertexDistance = 0.1f;
    [SerializeField] private Gradient color;

    [SerializeField] private float MissDistance = 100f;
    [SerializeField] private float SimulationSpeed = 100f;
}
