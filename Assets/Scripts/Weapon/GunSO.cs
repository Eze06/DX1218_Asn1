using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Pool;

[CreateAssetMenu(menuName="Weapon/Gun")]
public class GunSO : ScriptableObject
{
    [Header("Stats SO")]
    [SerializeField] GunStats gunStats;
    [SerializeField] BulletTrailStats bulletTrail;

    [Header("Gun Headers")]
    [SerializeField] GameObject ModelPrefab;
    [SerializeField] private string Name;

    
    private MonoBehaviour monoBehaviour;
    private ParticleSystem ShootSystem;
    private ObjectPool<TrailRenderer> TrailPool;
    private GameObject Model;

    private float ShootTimer;

    void Create(Transform parent, MonoBehaviour monoBehaviour) //Reset all variables for Scriptable Object
    {
        this.monoBehaviour = monoBehaviour;
        TrailPool = new ObjectPool<TrailRenderer>(CreateTrail);
        Model = GameObject.Instantiate(ModelPrefab);
        ShootTimer = 0;
        ShootSystem = Model.GetComponentInChildren<ParticleSystem>();
        
    }

    void Shoot(Camera FPScam)
    {
        if (ShootTimer >= 0)
        {
            ShootTimer -= Time.deltaTime;
        }
        else
        {
            //Shoot
            ShootTimer = gunStats.FireRate;
            ShootSystem.Play();

            Vector3 ShootDirection = FPScam.transform.forward;

            if (Physics.Raycast(FPScam.transform.position, ShootDirection, out RaycastHit hitInfo, float.MaxValue, gunStats.HitMask))
            {
                monoBehaviour.StartCoroutine(PlayTrail(ShootSystem.transform.position, hitInfo.point, hitInfo));
            }
            else
            {
                monoBehaviour.StartCoroutine(PlayTrail(ShootSystem.transform.position, ShootDirection * bulletTrail.MissDistance, new RaycastHit()));

            }

        }
    }

    private IEnumerator PlayTrail(Vector3 StartPos, Vector3 EndPos, RaycastHit Hit)
    {
        TrailRenderer instance = TrailPool.Get();
        instance.gameObject.SetActive(true);
        yield return null;

        instance.emitting = true;
        float distance = Vector3.Distance(StartPos, EndPos);
        float remainingDist = distance;
        while(remainingDist > 0.0f)
        {
            instance.transform.position = Vector3.Lerp(StartPos, EndPos, Mathf.Clamp01(1 - (remainingDist / distance)));
            yield return null;
        }

        instance.transform.position = EndPos;

        if(Hit.collider != null)
        {
            //Do impact effect
        }

        yield return new WaitForSeconds(bulletTrail.Duration);
        yield return null;
        instance.emitting = false;
        instance.gameObject.SetActive(false);
        TrailPool.Release(instance);
    }

    private TrailRenderer CreateTrail()
    {
        GameObject instance = new GameObject("Bullet Trail");
        TrailRenderer trail = instance.AddComponent<TrailRenderer>();
        trail.colorGradient = bulletTrail.color;
        trail.material = bulletTrail.material;
        trail.widthCurve = bulletTrail.WidthCurve;
        trail.time = bulletTrail.Duration;
        trail.minVertexDistance = bulletTrail.MinVertexDistance;

        trail.emitting = false;
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        return trail;
    }

}
