using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : Gun
{
    public override string GetActionStr(PlayerStateParam stateParam)
    {
        base.GetActionStr(stateParam);
        //Debug.Log($"[Rifle]GetActionStr, stateParam.isReloading={stateParam.isReloading}");
        string str = string.Empty;
        if (stateParam.isFiring)
        {
            if(CanDoAttack())
                str = WeaponActionDefine.Fire;
        }else if (stateParam.isReloading)
        {
            if(CanReload())
                str = WeaponActionDefine.Reload;
        }

        return str;
    }
}
