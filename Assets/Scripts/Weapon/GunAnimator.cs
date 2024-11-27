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

    [SerializeField] private Transform weaponParent;
    private Vector3 kickBackEndPos;
    private Vector3 currentKickBackPos;


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

    [Header("Weapon Bobbng Variable")]
    public bool doWeaponBobbing = true;
    public float speedCurve; 

    private float curveSin { get => Mathf.Sin(speedCurve); }
    private float curveCos { get => Mathf.Cos(speedCurve); }

    [SerializeField] private Vector3 travelLimit = Vector3.one * 0.025f;
    [SerializeField] private Vector3 bobLimit = Vector3.one * 0.1f;
    [SerializeField] private Vector3 bobRotMult;

    private Vector3 bobPos;
    private Vector3 bobRotation;
    private Vector3 currentBobPos;
    private Quaternion currentBobRot;

    private void Update()
    {
        kickBackEndPos = Vector3.Lerp(kickBackEndPos, Vector3.zero, kickBackReturnSpeed * Time.deltaTime);
        currentKickBackPos = Vector3.Slerp(currentKickBackPos, kickBackEndPos, kickBackSnappiness * Time.deltaTime);
        if (doWeaponSway)
        {
            currentSwayPos = Vector3.Lerp(currentSwayPos, swayPos, Time.deltaTime * swaySmooth);
            currentSwayRotation = Quaternion.Slerp(currentSwayRotation, Quaternion.Euler(swayRotation), Time.deltaTime * smoothRot);
        }
        if(doWeaponBobbing)
        {
            currentBobPos = Vector3.Lerp(currentBobPos, bobPos, Time.deltaTime * swaySmooth);
            currentBobRot = Quaternion.Slerp(currentBobRot, Quaternion.Euler(bobRotation), Time.deltaTime * smoothRot);
        }

        weaponParent.localPosition = currentKickBackPos + currentSwayPos + currentBobPos;
        weaponParent.localRotation = currentSwayRotation * currentBobRot;



    }
    public void UpdateKickBack(float kickBackAmount)
    {
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
    public void gunBob()
    {

    }
}
