using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Gun : Weapon
{
    protected int clampLeftAmmo;//弹夹剩余子弹
    protected int curAmmo;//当前剩余总子弹
    protected int clampCapacity;//弹夹容量
    protected int maxCapacity;//总的子弹数量
    protected int animSpeed;//连射动画倍速
    public int ClampLeftAmmo
    {
        get
        {
            TryFillData();
            return clampLeftAmmo;
        }
        set
        {
            var change = value - clampLeftAmmo;
            clampLeftAmmo = value;
            if(change < 0)//弹夹子弹变少了，总数量才会变
                curAmmo += change;
        }
    }

    public int CurAmmo
    {
        get { return curAmmo; }
    }
    
    public override bool CanDoAttack()
    {
        if (!PreCheck())
            return false;
        
        if (curAmmo <= 0 || clampLeftAmmo <= 0)
            return false;

        return true;
    }

    public virtual bool CanReload()
    {
        if (!PreCheck())
            return false;

        if (Owner == null)
        {
            Debug.LogError($"[Gun] gun={gameObject.name}'Owner is null, but try to reload!");
            return false;
        }

        var pData = Owner.RpPlayerData.Value;
        var weaponData = pData.weaponData;
        switch (this.type)
        {
            case WeaponTypeEnum.Rifle:
                return weaponData.clampRifleLeftAmmo < clampCapacity && weaponData.curRifleAmmo != maxCapacity;
                break;
            case WeaponTypeEnum.Pistol:
                return weaponData.clampPistolLeftAmmo < clampCapacity && weaponData.curPistolAmmo != maxCapacity;
                break;
            default:
                Debug.LogError($"[Gun] gun={gameObject.name}'s type:{type} is not gun, but try to reload!");
                break;
        }

        return false;
    }
    
    public override void Attack()
    {
        if(!CanDoAttack())
            return;

        if (clampLeftAmmo == 0)
        {
            Reload();
            return;
        }

        ClampLeftAmmo -= 1;
    }

    public virtual void Reload()
    {
        ClampLeftAmmo += Math.Max(0, clampCapacity - ClampLeftAmmo);
    }
    
    protected bool PreCheck()
    {
        TryFillData();
        if (maxCapacity == 0)
        {
            Debug.LogError($"[Gun] gun={Config.weaponName}的最大子弹容量={maxCapacity}！");
            return false;
        }

        if (clampCapacity == 0)
        {
            Debug.LogError($"[Gun] gun={Config.weaponName}的弹夹容量={maxCapacity}！");
            return false;
        }

        return true;
    }

    protected override void TryFillData()
    {
        base.TryFillData();
        if (maxCapacity != 0)
            return;

        var config = Config;
        clampCapacity = (int)config.clampCapacity;
        maxCapacity = (int)config.maxCapacity;
        curAmmo = maxCapacity;
        clampLeftAmmo = clampCapacity;
    }
}
