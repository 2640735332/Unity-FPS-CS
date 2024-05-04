using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public struct PlayerSettingData : INetworkSerializable
{
    public float mouseSensity;//鼠标灵敏度
    public float windowSensity;// 开镜灵敏度
    
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref mouseSensity);
        serializer.SerializeValue(ref windowSensity);
    }
}
