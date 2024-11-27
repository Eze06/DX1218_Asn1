using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SurfaceManager : MonoBehaviour
{
    public static SurfaceManager Instance { get; private set; }

    private void Awake()
    {
        
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

    }

    public void DoSurfaceEffect(Surface surface, Vector3 hitPoint, Vector3 normal)
    {
        if(surface.impactEffect != null)
        {
            //Spawn Impact effect at hitlocation
            GameObject TempBullet = ObjectPoolManager.SpawnObject(surface.impactEffect, hitPoint, Quaternion.LookRotation(normal));
            StartCoroutine(ReleaseSurfaceEffect(TempBullet, surface.impactEffectDuration));
        }
    }


    private IEnumerator ReleaseSurfaceEffect(GameObject effect, float delay)
    {
        yield return new WaitForSeconds(delay);
        ObjectPoolManager.ReturnObjectToPool(effect);

    }

}
