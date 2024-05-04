using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class PlayerCharacHandler : PlayerBaseHandler
{
    private CharacterController characterController;
    private float jumpUpSpeed = 3f;
    private Vector3 verticalSpeed;
    private Vector3 rotatedMove;
    
    public PlayerCharacHandler(PlayerController playerController, CharacterController characControl) : base(playerController)
    {
        characterController = characControl;
        verticalSpeed = Vector3.zero;
        rotatedMove = Vector3.zero;
    }

    public override void HandlePlayerState(PlayerStateParam stateParam)
    {
        base.HandlePlayerState(stateParam);
        
        RotateMove(stateParam);

        var moveAndFireLock = pControl.IsMoveAndFireLock();
        if (!moveAndFireLock)
        {
            var move = rotatedMove;
            characterController.Move(move * Time.deltaTime * stateParam.speed);
            verticalSpeed.y += Physics.gravity.y * Time.deltaTime;
            characterController.Move(verticalSpeed * Time.deltaTime);
        }
        
        if (characterController.isGrounded)
        {
            stateParam.move.y = 0;
            verticalSpeed.y = 0;
            stateParam.isJumpingUp = false;
            stateParam.isFalling = false;
        }
        else
        {
            if (verticalSpeed.y > 0)
                stateParam.isJumpingUp = true;
            else
                stateParam.isFalling = true;
        }
        
        if (stateParam.isJump && !stateParam.isJumpingUp && !stateParam.isFalling)
            verticalSpeed.y += jumpUpSpeed;
        
        stateParam.isJump = false;
        stateParam.isGrounded = characterController.isGrounded;
    }
    
    private void RotateMove(PlayerStateParam stateParam)
    {
        var quaternion = pControl.CameraHandler.PlayerCamera.transform.rotation;
        rotatedMove = quaternion * stateParam.move;
    }
}
