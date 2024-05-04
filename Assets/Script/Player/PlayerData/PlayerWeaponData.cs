using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[Serializable]
public struct PlayerWeaponData : INetworkSerializable
{
    // public Weapon CurWeapon;
    //
    // public Weapon RifleWeapon;
    // public Weapon PistolWeapon;
    // public Weapon KinfeWeapon;
    // public List<Weapon> ThrowObjects;
    public uint rifleID;
    public uint clampRifleLeftAmmo;
    public uint curRifleAmmo;
    
    public uint pistolID;
    public uint clampPistolLeftAmmo;
    public uint curPistolAmmo;

    public uint kinfeID;
    public uint[] throwIDList;
    
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref rifleID);
        serializer.SerializeValue(ref clampRifleLeftAmmo);
        serializer.SerializeValue(ref curRifleAmmo);
        
        serializer.SerializeValue(ref pistolID);
        serializer.SerializeValue(ref clampPistolLeftAmmo);
        serializer.SerializeValue(ref curPistolAmmo);
        
        serializer.SerializeValue(ref kinfeID);
        if (throwIDList != null)
        {
            serializer.SerializeValue(ref throwIDList);
        }
    }
}
