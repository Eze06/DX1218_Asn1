using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{

    public GunData gunData;
    public AmmoData ammoData;
    // Start is called before the first frame update
    private float BufferTime = 0;
    private bool canPickUp = false;

    private void Update()
    {
        if (BufferTime >= 1)
        {
            canPickUp = true;
            return;
        }

        BufferTime += Time.deltaTime;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && canPickUp)
        {
            other.GetComponent<InventoryController>().PickUpItem(this); //Handle Pickup
        }
    }
}
