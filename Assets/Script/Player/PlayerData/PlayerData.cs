using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;

[Serializable]
public struct PlayerData : INetworkSerializable
{
    public uint curWeaponID;
    public uint modelID;
    public uint health;
    public GameCamp camp;
    public uint spawnPos;//出生点位
    
    public PlayerUserData userData;
    public PlayerWeaponData weaponData;
    public PlayerSettingData settingData;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref curWeaponID);
        serializer.SerializeValue(ref modelID);
        serializer.SerializeValue(ref health);
        serializer.SerializeValue(ref camp);
        serializer.SerializeValue(ref spawnPos);
        settingData.NetworkSerialize(serializer);
        weaponData.NetworkSerialize(serializer);
        userData.NetworkSerialize(serializer);
    }

    public bool IsLockInput()
    {
        return health <= 0;
    }
}
