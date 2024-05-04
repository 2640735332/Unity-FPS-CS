using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[Serializable]
public struct PlayerInputParam : INetworkSerializable
{
    public Vector3 move;
    public Vector3 mouseMove;
    public bool jump;
    public bool fire;
    public bool rightFire;
    public bool walkQuiet;
    public bool reload;
    public float scrollWheel;

    /// <summary>
    /// 重置所有值
    /// </summary>
    public void ResetAllParam()
    {
        move = Vector3.zero;
        mouseMove = Vector3.zero;
        jump = false;
        fire = false;
        walkQuiet = false;
        reload = false;
        rightFire = false;
        scrollWheel = 0;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref move);
        serializer.SerializeValue(ref mouseMove);
        serializer.SerializeValue(ref jump);
        serializer.SerializeValue(ref fire);
        serializer.SerializeValue(ref walkQuiet);
        serializer.SerializeValue(ref reload);
        serializer.SerializeValue(ref rightFire);
        serializer.SerializeValue(ref scrollWheel);
    }
}
