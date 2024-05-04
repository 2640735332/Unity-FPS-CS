using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBaseHandler
{
    protected PlayerController pControl;

    public PlayerBaseHandler(PlayerController pControl)
    {
        this.pControl = pControl;
    }

    public virtual void HandlePlayerInput(PlayerInputParam inputParam)
    {
        
    }

    public virtual void HandlePlayerState(PlayerStateParam stateParam)
    {
        
    }
    
    public virtual void HandlePlayerState(PlayerStateParam stateParam, PlayerInputParam inputParam)
    {
        
    }

    
    /// <summary>
    /// 重置状态
    /// </summary>
    public virtual void Reset()
    {
        
    }
}
