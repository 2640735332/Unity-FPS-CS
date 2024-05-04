using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerInputKey
{
    public KeyCode MoveForward = KeyCode.W;
    public KeyCode MoveBack = KeyCode.S;
    public KeyCode MoveLeft = KeyCode.A;
    public KeyCode MoveRight = KeyCode.D;
    public KeyCode LeftFire = KeyCode.Mouse0;
    public KeyCode RightFire = KeyCode.Mouse2;
    public KeyCode WeaponChange = KeyCode.Mouse1;
    public KeyCode Reload = KeyCode.R;
    public KeyCode MoveQuietly = KeyCode.LeftShift;
    public KeyCode Jump = KeyCode.Space;
    public KeyCode Back = KeyCode.Escape;
    public KeyCode ScoreTable = KeyCode.Tab;
    public KeyCode Store = KeyCode.B;
    public KeyCode DropWeapon = KeyCode.G;
    public KeyCode TakeWeapon = KeyCode.E;
    public float mouseSensity = 1;
}
