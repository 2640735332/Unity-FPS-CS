using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public struct PlayerStateParam : INetworkSerializable
{
    private PlayerInputParam curInputParam;
    public PlayerInputParam CurInputParam
    {
        set
        {
            curInputParam = value;
            ParseInput();
        }
        get { return curInputParam; }
    }

    public Vector3 move;
    public Vector3 mouseMove;
    public bool isJump;
    public bool isJumpingUp;
    public bool isFalling;
    public bool isFiring;
    public bool isRightFiring;
    public bool isWalkQuietly;
    public bool isGrounded;
    public bool isReloading;
    public float speed;
    public float scrollWheel;
    //public string actionStr;
    
    public void ResetAllState()
    {
        curInputParam.ResetAllParam();
        move = Vector3.zero;
        mouseMove = Vector3.zero;
        isJump = false;
        isJumpingUp = false;
        isFalling = false;
        isFiring = false;
        isRightFiring = false;
        isWalkQuietly = false;
        isGrounded = false;
        isReloading = false;
        scrollWheel = 0;
        //actionStr = string.Empty;
        speed = 1;
    }
    
    void ParseInput()
    {
        this.move = curInputParam.move;
        this.mouseMove = curInputParam.mouseMove;
        this.isJump = curInputParam.jump;
        this.isFiring = curInputParam.fire;
        this.isRightFiring = curInputParam.rightFire;
        this.isWalkQuietly = curInputParam.walkQuiet;
        this.isReloading = curInputParam.reload;
        this.scrollWheel = curInputParam.scrollWheel;
        speed = 4;
    }
    
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        curInputParam.NetworkSerialize(serializer);
        serializer.SerializeValue(ref move);
        serializer.SerializeValue(ref mouseMove);
        serializer.SerializeValue(ref isJump);
        serializer.SerializeValue(ref isJumpingUp);
        serializer.SerializeValue(ref isFalling);
        serializer.SerializeValue(ref isFiring);
        serializer.SerializeValue(ref isRightFiring);
        serializer.SerializeValue(ref isWalkQuietly);
        serializer.SerializeValue(ref isGrounded);
        serializer.SerializeValue(ref isReloading);
        serializer.SerializeValue(ref speed);
        serializer.SerializeValue(ref scrollWheel);
        // if(!string.IsNullOrEmpty(actionStr))
        //     serializer.SerializeValue(ref actionStr);
    }
}
