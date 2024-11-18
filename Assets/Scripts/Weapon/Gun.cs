using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Gun : MonoBehaviour
{
    [SerializeField] AmmoData ammoData;
    public GunData gunData;
    [SerializeField] BulletTrailStats bulletTrail;
    ObjectPool<TrailRenderer> TrailPool;

    [SerializeField] ParticleSystem ShootSystem;

    private void Awake()
    {
        TrailPool = new ObjectPool<TrailRenderer>(CreateTrail);
    }

    public void SwitchFireMode()
    {

    }

    public void Shoot(Transform parent)
    {

        //Check if gun is in primary or secondary fire mode
        //Check what is the fire mode
        //Shoot

        switch (gunData)
        {

        }

        Vector3 ShootDirection = parent.forward;

        if (Physics.Raycast(parent.position, ShootDirection, out RaycastHit hitInfo, float.MaxValue, gunData.HitMask))
        {
            StartCoroutine(PlayTrail(ShootSystem.transform.position, hitInfo.point, hitInfo));
        }
        else
        {
            StartCoroutine(PlayTrail(ShootSystem.transform.position, ShootDirection * bulletTrail.MissDistance, new RaycastHit()));

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
        while (remainingDist > 0.0f)
        {
            instance.transform.position = Vector3.Lerp(StartPos, EndPos, Mathf.Clamp01(1 - (remainingDist / distance)));
            yield return null;
        }

        instance.transform.position = EndPos;

        if (Hit.collider != null)
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
