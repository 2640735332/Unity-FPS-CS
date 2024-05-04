using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputHandler : PlayerBaseHandler
{
    private PlayerInputParam inputParam;
    private PlayerInputKey inputKey;
    
    public PlayerInputHandler(PlayerController playerController) : base(playerController)
    {
        inputParam = new PlayerInputParam();
        inputKey = new PlayerInputKey();
    }
    
    /// <summary>
    /// 获取玩家输入参数
    /// </summary>
    /// <returns></returns>
    public PlayerInputParam GetPlayerInput()
    {
        inputParam.ResetAllParam();
        inputParam.move.x = Input.GetAxis("Horizontal");
        inputParam.move.z = Input.GetAxis("Vertical");

        inputParam.mouseMove.x = Input.GetAxis("Mouse X");
        inputParam.mouseMove.z = Input.GetAxis("Mouse Y");

        inputParam.scrollWheel = Input.GetAxis("Mouse ScrollWheel");
        
        if (Input.GetKeyDown(inputKey.Jump))
            inputParam.jump = true;

        if (Input.GetKeyDown(inputKey.LeftFire) || Input.GetKey(inputKey.LeftFire))
            inputParam.fire = true;

        if (Input.GetKeyDown(inputKey.MoveQuietly))
            inputParam.walkQuiet = true;

        if (Input.GetKeyDown(inputKey.Reload))
            inputParam.reload = true;

        if (Input.GetKeyDown(inputKey.RightFire))
            inputParam.rightFire = true;
        
        return this.inputParam;
    }
}
