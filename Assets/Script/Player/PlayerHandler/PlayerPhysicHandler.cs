using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPhysicHandler : PlayerBaseHandler
{
    public PlayerPhysicHandler(PlayerController pControl) : base(pControl)
    {
        
    }

    public void UnderAttack(Weapon weapon)
    {
        float damage = weapon.Damage;
        PlayerData data = pControl.RpPlayerData.Value;
        uint health = data.health;
        
        data.health = (uint)Math.Floor(Math.Max(0, data.health - damage));
        Debug.Log($"[PlayerPhysicHandler] client{pControl.OwnerClientId} isUnderAttack, health{health}-damage{damage}=newhealth{health - damage}");
        pControl.RpPlayerData.Value = data;
        if (data.health < 1)
            Notifier.Dispatch(EventEnum.OnlySvr_OnPlayerDeath, data);
    }
}
