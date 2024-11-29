using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Gun : MonoBehaviour
{
    [SerializeField] AmmoData ammoData;
    public GunData gunData;

    [SerializeField] ParticleSystem muzzleFlash;
    private float nextFireTime;
    public int CurrentRounds;

    [HideInInspector] public CameraAnimation cameraAnimation;
    public GunAnimator gunAnimator;
    [SerializeField] public Sprite GunIcon;    
    
    bool doneBurst;

    public void Init()
    {
        Debug.Log("Awake,:" +  this.name);
        cameraAnimation = GameObject.Find("CameraAnimator").GetComponent<CameraAnimation>();
        gunAnimator = GetComponent<GunAnimator>();
        nextFireTime = 0;
        doneBurst = true;

        cameraAnimation.snappiness = gunData.snappiness;
        cameraAnimation.returnSpeed = gunData.returnSpeed;

        CurrentRounds = gunData.RoundsPerMag;

    }

    public void ADS()
    {
        gunAnimator.ADS();
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

    public void GunBob(float moveSpeed)
    {
        gunAnimator.GunBob(cameraAnimation.bobFreq, cameraAnimation.bobAmp, moveSpeed);
    }

    public void Shoot(Camera fpsCamera, bool burst = false)
    {
        if (Time.time <= nextFireTime && !burst)
            return;

        if (CurrentRounds <= 0)
            return;

        CurrentRounds--;

        nextFireTime = Time.time + gunData.FireRate;

        Ray ray = fpsCamera.ViewportPointToRay(
        new Vector3(0.5f, 0.5f, 0.0f));

        cameraAnimation.RecoilFire(gunData.Spread);

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

    }

    public IEnumerator Burst(Camera fpsCamera)
    {

        if (Time.time <= nextFireTime || !doneBurst)
            yield break;
        nextFireTime += Time.time + gunData.FireRate / 2f  ;

        for (int i = 0; i < gunData.BulletsPerBurst;i++)
        {
            doneBurst = false;
            if (CurrentRounds <= 0)
                break;

            Shoot(fpsCamera , burst: true);
            yield return new WaitForSeconds(gunData.TimeBetweenBurst);
            
        }
        doneBurst = true;
    }

    public void Drop(Transform FPSCamera)
    {
        GameObject droppableObject = Instantiate(gunData.droppablePrefab, FPSCamera.transform.position + FPSCamera.transform.forward * 1f, Quaternion.identity) ;
        droppableObject.GetComponent<Rigidbody>().AddForce(FPSCamera.transform.forward * 5f,ForceMode.Impulse);
        
    }

    public void Reload(InventoryAmmo invAmmo)
    {
        int totalAmmo = invAmmo.Amount;
        if(totalAmmo >= gunData.RoundsPerMag)
        {
            int ammoToRemove = gunData.RoundsPerMag - CurrentRounds;
            invAmmo.Amount -= ammoToRemove;
            CurrentRounds = gunData.RoundsPerMag;
        }
        else
        {
            int AmmoNeeded = gunData.RoundsPerMag - CurrentRounds;
            if(invAmmo.Amount < AmmoNeeded)
            {
                CurrentRounds += invAmmo.Amount;
                invAmmo.Amount = 0;
            }
            else
            {
                invAmmo.Amount -= AmmoNeeded;
                CurrentRounds += AmmoNeeded;
            }
        }
    }

    public void ShotGunShot(Camera fpsCamera)
    {
        muzzleFlash.Play();
        for(int i = 0; i < gunData.NumBulletsPerShot; i++)
        {
            if (CurrentRounds <= 0)
                break;

            CurrentRounds--;


            Vector3 dir = fpsCamera.transform.forward;
            dir += new Vector3(Random.Range(-gunData.shotgunSpread.x, gunData.shotgunSpread.x),
                               Random.Range(-gunData.shotgunSpread.y, gunData.shotgunSpread.y),
                               0);
            dir.Normalize();

            cameraAnimation.RecoilFire(gunData.Spread);


            if (Physics.Raycast(fpsCamera.transform.position, dir, out RaycastHit hitInfo, float.MaxValue))
            {
                if (hitInfo.collider.GetComponent<Surface>())
                {
                    SurfaceManager.Instance.DoSurfaceEffect(hitInfo.collider.GetComponent<Surface>(), hitInfo.point, hitInfo.normal);
                }
            }


        }
    }


}
