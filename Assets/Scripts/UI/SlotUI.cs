using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SlotUI : MonoBehaviour
{

    [SerializeField] Image gunImage;
    [SerializeField] TMP_Text ammoCount;
    [SerializeField] Image isActive;
    public void Display(bool bDisplay, Gun gun = null)
    {
        gameObject.SetActive(bDisplay);
        if (gun != null)
        {
            gunImage.sprite = gun.GunIcon;
            ammoCount.text = gun.CurrentRounds.ToString();
        }
        
    }

    public void Select()
    {
        this.isActive.color = Color.white;
    }   

    public void Deselect()
    {
        this.isActive.color = Color.gray;
    }

}
