using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public struct PlayerUserData : INetworkSerializable
{
    public ulong userID;
    public string userName;
    
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref userID);
        if(!string.IsNullOrEmpty(userName))
            serializer.SerializeValue(ref userName);
    }
}
