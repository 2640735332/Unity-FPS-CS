using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CommonAnimEvent : MonoBehaviour
{
    public void OnFire()
    {
        var parentGo = transform.parent.gameObject;
        PlayerController pControl = parentGo.GetComponent<PlayerController>();
        if (pControl.IsOwner)
        {
            pControl.OnWeaponFire();
        }
    }

    public void OnReload()
    {
        var parentGo = transform.parent.gameObject;
        PlayerController pControl = parentGo.GetComponent<PlayerController>();
        if (pControl.IsOwner)
        {
            pControl.OnWeaponReload();
        }
    }
}
