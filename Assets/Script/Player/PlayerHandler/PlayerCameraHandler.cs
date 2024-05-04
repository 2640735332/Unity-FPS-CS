using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraHandler : PlayerBaseHandler
{
    private Camera playerCamera;
    public Camera PlayerCamera => playerCamera;
    
    private float clampAngle = 30;
    private float verticalRotation;
    private float horizontalRotation;
    
    public PlayerCameraHandler(PlayerController pControl) : base(pControl)
    {
        this.playerCamera = pControl.GetComponentInChildren<Camera>();
    }
    
    public override void HandlePlayerState(PlayerStateParam stateParam)
    {
        var mouseMove = stateParam.mouseMove;
        PlayerData playerData = pControl.RpPlayerData.Value;
        float sensitivity = playerData.settingData.mouseSensity;
        
        horizontalRotation += mouseMove.x * sensitivity * Time.deltaTime;
        pControl.transform.rotation = Quaternion.Euler(0, horizontalRotation,0);
        
        verticalRotation += -mouseMove.z * sensitivity * Time.deltaTime;
        verticalRotation = Mathf.Clamp(verticalRotation, -clampAngle, clampAngle);
        pControl.CameraHandler.PlayerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }
}
