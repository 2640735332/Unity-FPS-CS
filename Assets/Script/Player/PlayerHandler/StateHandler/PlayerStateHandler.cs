using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStateHandler : PlayerBaseHandler
{
    private PlayerStateParam stateParam;
    public PlayerStateHandler(PlayerController playerController) : base(playerController)
    {
        stateParam = new PlayerStateParam();
    }

    public PlayerStateParam ParsePlayerInput(PlayerInputParam inputParam)
    {
        stateParam.CurInputParam = inputParam;
        return stateParam;
    }
    
    
}
