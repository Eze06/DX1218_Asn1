using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRecoil : MonoBehaviour
{
    // Start is called before the first frame update

    private Vector3 currentRotation;
    private Vector3 targetRotation;

    [HideInInspector] public float returnSpeed = 5f;
    [HideInInspector] public float snappiness = 1.0f;


    // Update is called once per frame
    void Update()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(currentRotation);    
    }

    public void RecoilFire(Vector3 Recoil)
    {
        targetRotation += new Vector3(Recoil.x, Random.Range(-Recoil.y, Recoil.y), Random.Range(-Recoil.z, Recoil.z));
    }
}
