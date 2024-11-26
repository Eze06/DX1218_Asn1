using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

[DisallowMultipleComponent]
public class GunAnimator : MonoBehaviour
{

    [SerializeField] private Transform kickBackParent;
    private Vector3 kickBackEndPos;
    private Vector3 CurrentKickBackPos;

    [Header("Kickback Variables")]
    [SerializeField]private float kickBackReturnSpeed = 2;
    [SerializeField] private float kickBackSnappiness = 6;
    private void Update()
    {
        kickBackEndPos = Vector3.Lerp(kickBackEndPos, Vector3.zero, kickBackReturnSpeed * Time.deltaTime);
        CurrentKickBackPos = Vector3.Slerp(CurrentKickBackPos, kickBackEndPos, kickBackSnappiness * Time.deltaTime);

        kickBackParent.localPosition = CurrentKickBackPos;
    }
    public void UpdateKickBack(float kickBackAmount)
    {
        kickBackEndPos += new Vector3(0,0f, -kickBackAmount);

    }
}
