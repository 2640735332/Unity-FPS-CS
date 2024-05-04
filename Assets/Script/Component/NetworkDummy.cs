using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkDummy : NetworkBehaviour
{
    private Action callbackFunc;
    
    public override void OnNetworkSpawn()
    {
        callbackFunc?.Invoke();
    }

    public void RegistNetSpawn(Action callback)
    {
        callbackFunc += callback;
    }

    public void UnRegistNetSpawn()
    {
        callbackFunc -= callbackFunc;
    }
}
