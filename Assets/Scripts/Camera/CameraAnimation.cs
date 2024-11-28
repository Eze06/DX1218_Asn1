using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnimation : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector3 currentRotation;
    private Vector3 targetRotation;
    private Vector3 startPos;

    [SerializeField]
    private Transform FPScamera;

    [HideInInspector] public float returnSpeed = 5f;
    [HideInInspector] public float snappiness = 1.0f;

    [Header("Camera Shake Variables")]
    [Range(0, 1)]
    [SerializeField] private float trauma;
    [SerializeField] private float traumaMult;
    [SerializeField] private float traumaMag;
    private float cameraShakeTimer = 0f;
    private Vector3 newPos = Vector3.zero;

    [Header("Camera Bob Variables")]
    [Range(0, 10)]
     public float bobFreq;
     public float bobAmp;

    float timeCounter;

    [Header("Camera FOV Handler")]
    public bool zoomSprint = false;
    public bool zoomADS = false;
    private float sprintFOV;
    private float ADSFOV;
    private float normalFOV;
    private float currentFOV;

    private float adsFOVTransition = 5f;
    private float movementFOVTransition = 2f;
    [SerializeField] private float sprintFOVMultiplier = 0.8f;
    [SerializeField] private float ADSFOVMultiplier = 0.5f;

    public float Trauma
    {
        get { return trauma; } set { trauma = Mathf.Clamp01(value); }
    }

    private void Awake()
    {
        zoomADS = false;
        zoomSprint = false;
        startPos = FPScamera.localPosition;

        currentFOV = normalFOV = Camera.main.fieldOfView;
        sprintFOV = normalFOV * sprintFOVMultiplier;
        ADSFOV = normalFOV * ADSFOVMultiplier;
    }

    // Update is called once per frame
    void Update()
    {
        HandleCameraFOV();

        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.deltaTime);

        transform.localRotation = Quaternion.Euler(currentRotation);

        if (cameraShakeTimer > 0 && trauma > 0)
        {
            cameraShakeTimer -= Time.deltaTime;
            timeCounter += Time.deltaTime * trauma * traumaMult;
            newPos = GetVec3() * traumaMag;
            transform.localRotation = Quaternion.Euler(newPos + currentRotation);
        }
        else
        {
            newPos = Vector3.Lerp(newPos, Vector3.zero, Time.deltaTime);
            transform.localRotation = Quaternion.Euler(newPos + currentRotation);
        }

        if(FPScamera.localPosition != startPos)
        {
            FPScamera.localPosition = Vector3.Lerp(FPScamera.localPosition, startPos, 1 * Time.deltaTime);
        }
    }

    private void HandleCameraFOV()
    {
        if (zoomSprint)
        {
            currentFOV = Mathf.Lerp(currentFOV, sprintFOV, movementFOVTransition * Time.deltaTime);
        }
        else if (zoomADS)
        {
            currentFOV = Mathf.Lerp(currentFOV, ADSFOV, adsFOVTransition * Time.deltaTime);
        }
        else
        {
            currentFOV = Mathf.Lerp(currentFOV, normalFOV, movementFOVTransition * Time.deltaTime);

        }

        Camera.main.fieldOfView = currentFOV;
    }

    public void RecoilFire(Vector3 Recoil)
    {
        targetRotation += new Vector3(Recoil.x, Random.Range(-Recoil.y, Recoil.y), Random.Range(-Recoil.z, Recoil.z));
    }

    public void CameraShake()
    {
        cameraShakeTimer = 1f;
    }

    float GetFloat(float seed)
    {
        return Mathf.PerlinNoise(seed, timeCounter) - 0.5f * 2;
    }

    Vector3 GetVec3()
    {
        return new Vector3
            (GetFloat(1), GetFloat(10), 0
            ) ;
    }

    public void HeadBob(float frequencyMult)
    {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sin(Time.time * bobFreq * frequencyMult) * bobAmp;
        pos.x += Mathf.Cos(Time.time * bobFreq / 2 * frequencyMult) * bobAmp * 2;
        

        FPScamera.localPosition += pos;
    }
}
