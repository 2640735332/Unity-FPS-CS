using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Gun
{
    public override string GetActionStr(PlayerStateParam stateParam)
    {
        base.GetActionStr(stateParam);
        string str = string.Empty;
        if (stateParam.isFiring)
        {
            str = WeaponActionDefine.Fire;
        }else if (stateParam.isReloading)
        {
            str = WeaponActionDefine.Reload;
        }

        return str;
    }
}
