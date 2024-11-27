using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Gun : MonoBehaviour
{
    [SerializeField] AmmoData ammoData;
    public GunData gunData;
    ObjectPool<GameObject> TrailPool;

    [SerializeField] ParticleSystem muzzleFlash;

    private float nextFireTime;
    public int CurrentRounds;

    private CameraAnimation cameraAnimation;
    [HideInInspector] public GunAnimator gunAnimator;

    private void Awake()
    {
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
        muzzleFlash.Play();
        if (Physics.Raycast(ray, out RaycastHit hitInfo, float.MaxValue))
        {
            if(hitInfo.collider.GetComponent<Surface>())
            {
                SurfaceManager.Instance.DoSurfaceEffect(hitInfo.collider.GetComponent<Surface>(), hitInfo.point, hitInfo.normal);
            }
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

    
}
