using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

[DisallowMultipleComponent]
public class GunAnimator : MonoBehaviour
{
    public bool doKickBack = true;
    [SerializeField] private Transform weaponParent;
    private Vector3 kickBackEndPos;
    private Vector3 currentKickBackPos;

    private Vector3 startingPos;

    [Header("Kickback Variables")]
    [SerializeField]private float kickBackReturnSpeed = 2;
    [SerializeField] private float kickBackSnappiness = 6;

    [Header("Weapon Sway Variables")]
    public bool doWeaponSway = true;
    private Vector3 swayPos = Vector3.zero;
    private Vector3 swayRotation = Vector3.zero;

    private Vector3 currentSwayPos;
    private Quaternion currentSwayRotation;
    [SerializeField] private float step = 0.01f;
    [SerializeField] private float maxStepDistance = 0.6f;
    [SerializeField] private float rotationStep = 0.4f;
    [SerializeField] private float maxRotationStepDistance = 5f;

    float swaySmooth = 10f;
    float smoothRot = 12f;




    [Header("ADS Variables")]
    public bool DoADS = false;
    [SerializeField] private Transform adsPoint;
    [SerializeField] private Transform adsTarget;
    private Vector3 targetADSpoint;
    private Vector3 currentADSpoint;
    private Vector3 weaponFromADSTarget;
    [SerializeField] private float adsSpeed;

    [Header("Sprint Variable")]
    public bool doSprint;
    [SerializeField] private Vector3 SprintRotationTarget;
    private Quaternion CurrentSprintRotation;
    [SerializeField] private float transToSprintSpeed = 5f;

    private Vector3 bobPos;
    private Vector3 currentBobPos;


    private void Awake()
    {
        doKickBack = true;
        startingPos = weaponParent.localPosition;
        Vector3 weaponFromADSPoint = adsPoint.position - transform.position;
        weaponFromADSTarget = adsTarget.position - transform.position - weaponFromADSPoint;
    }

    private void Update()
    {
        
        if (doKickBack)
        {
            kickBackEndPos = Vector3.Lerp(kickBackEndPos, Vector3.zero, kickBackReturnSpeed * Time.deltaTime);
            currentKickBackPos = Vector3.Slerp(currentKickBackPos, kickBackEndPos, kickBackSnappiness * Time.deltaTime);
        }


        if (doWeaponSway)
        {
            currentSwayPos = Vector3.Lerp(currentSwayPos, swayPos, Time.deltaTime * swaySmooth);
            currentSwayRotation = Quaternion.Slerp(currentSwayRotation, Quaternion.Euler(swayRotation), Time.deltaTime * smoothRot);
        }

        if(doSprint)
        {
            CurrentSprintRotation = Quaternion.Slerp(CurrentSprintRotation, Quaternion.Euler(SprintRotationTarget), Time.deltaTime * transToSprintSpeed);
        }
        else
        {
            CurrentSprintRotation = Quaternion.Slerp(CurrentSprintRotation, Quaternion.Euler(Vector3.zero), Time.deltaTime * transToSprintSpeed);
        }

        targetADSpoint = Vector3.Lerp(targetADSpoint, startingPos, Time.deltaTime * adsSpeed);
        currentADSpoint = Vector3.Slerp(currentADSpoint, targetADSpoint, Time.deltaTime * adsSpeed);

        weaponParent.localPosition = currentKickBackPos + currentSwayPos + currentBobPos + currentADSpoint;
        weaponParent.localRotation = currentSwayRotation * CurrentSprintRotation;

    }

    public void ADS()
    {
        DoADS = true;
        targetADSpoint = weaponFromADSTarget;
        currentKickBackPos = Vector3.zero;
    }

    public void UpdateKickBack(float kickBackAmount)
    {
        if (!doKickBack)
            return;
        kickBackEndPos += new Vector3(0,0f, -kickBackAmount);

    }

    public void Sway(Vector3 mouseInput)
    {
        if (!doWeaponSway)
            return;

        Vector3 invertLook = mouseInput * -step;

        invertLook.x = Mathf.Clamp(invertLook.x, -maxStepDistance, maxStepDistance);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxStepDistance, maxStepDistance);

        swayPos = invertLook;
    }

    public void SwayRotation(Vector3 mouseInput)
    {
        if (!doWeaponSway)
            return;

        Vector3 invertLook = mouseInput * -rotationStep;

        invertLook.x = Mathf.Clamp(invertLook.x, -maxRotationStepDistance, maxRotationStepDistance);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxRotationStepDistance, maxRotationStepDistance);

     
        swayRotation = new Vector3(invertLook.y, invertLook.x, invertLook.x);
    }

    public void GunBob(float bobFreq, float bobAmp, float frequencyMult)
    {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sin(Time.time * bobFreq * frequencyMult) * bobAmp;
        pos.x += Mathf.Cos(Time.time * bobFreq / 2 * frequencyMult) * bobAmp * 2;

        currentBobPos = pos;
    }

}
