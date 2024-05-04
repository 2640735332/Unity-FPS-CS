using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Netcode;
using UnityEngine;

public class Weapon : NetworkBehaviour
{
    [SerializeField]
    private uint id;
    
    [HideInInspector]
    public string name;
    [HideInInspector]
    public float damage;
    [HideInInspector]
    public WeaponTypeEnum type;
    
    private PlayerController owner;

    public PlayerController Owner
    {
        get
        {
            return owner;
        }
        set
        {
            owner = value;
        }
    }

    public uint ID
    {
        get { return id; }
    }
    
    public string Name
    {
        get
        {
            TryFillData();
            return name;
        }
        set { name = value; }
    }
    
    public float Damage
    {
        get
        {
            TryFillData();
            return damage;
        }
        set { damage = value; }
    }
    
    public WeaponTypeEnum Type
    {
        get { return type; }
        set { type = value; }
    }
    
    public WeaponConfigData Config
    {
        get
        {
            var cfg = (WeaponConfigData)ConfigManager.Instance.WeaponConfig.GetCfg(id);;
            return cfg;
        }
    }

    protected virtual void TryFillData()
    {
        if(!string.IsNullOrEmpty(name))
            return;
        
        var config = (WeaponConfigData)ConfigManager.Instance.WeaponConfig.GetCfg(id);;
        name = config.weaponName;
        damage = config.damage;
        type = config.weaponType;
    }

    public virtual void Drop()
    {
    }

    public virtual void PickUp()
    {
        
    }


    /// <summary>
    /// 是否可以进行攻击
    /// </summary>
    /// <returns></returns>
    public virtual bool CanDoAttack()
    {
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public virtual void Attack()
    {
        
    }
    
    public virtual string GetActionStr(PlayerStateParam stateParam)
    {
        return string.Empty;
    }
}
