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

    private float nextFireTime;
    public int CurrentRounds;

    private CameraAnimation cameraAnimation;
    [HideInInspector] public GunAnimator gunAnimator;

    private void Awake()
    {
        TrailPool = new ObjectPool<TrailRenderer>(CreateTrail);
        cameraAnimation = GameObject.Find("CameraAnimator").GetComponent<CameraAnimation>();
        gunAnimator = GetComponent<GunAnimator>();
        nextFireTime = 0;
    }


    public void SwitchFireMode()
    {
        if(gunData.fireMode == GunData.FireMode.PRIMARY_FIRE)
        {
            gunData.fireMode = GunData.FireMode.SECONDARY_FIRE;
            return;
        }
        else
        {
            gunData.fireMode = GunData.FireMode.PRIMARY_FIRE;
            return;
        }
    }

    public void Shoot(Camera fpsCamera)
    {
        if (Time.time <= nextFireTime)
            return;

        nextFireTime = Time.time + gunData.FireRate;

        Ray ray = fpsCamera.ViewportPointToRay(
        new Vector3(0.5f, 0.5f, 0.0f));

        cameraAnimation.RecoilFire(gunData.Spread);
        cameraAnimation.snappiness = gunData.snappiness;
        cameraAnimation.returnSpeed = gunData.returnSpeed;

        cameraAnimation.CameraShake();

        gunAnimator.UpdateKickBack(gunData.kickBackAmt);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, float.MaxValue))
        {
            Debug.Log(hitInfo.point);
        }

        Debug.DrawLine(fpsCamera.transform.position, hitInfo.point, Color.red);
    }

    public void Burst(Camera fpsCamera)
    {
        for(int i = 0; i < gunData.BulletsPerBurst;i++)
        {
            if (CurrentRounds <= 0)
                break;   

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
