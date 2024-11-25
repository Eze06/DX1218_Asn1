using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class GunAnimator : MonoBehaviour
{
    private Transform kickBackParent;
    private Vector3 kickBackEndPos;



    private void UpdateKickBack(GunData gunData)
    {
        if (kickBackParent.localPosition == Vector3.zero && kickBackEndPos == Vector3.zero)
            return;

        //float speed;
        //if(kickBackEndPos == Vector3.zero)
        //{
        //    speed = gunData.kickBackReturnSpeed;
        //}
        //else
        //{
        //    speed = gunData.kickBackSpeed;
        //}
    }    
}
