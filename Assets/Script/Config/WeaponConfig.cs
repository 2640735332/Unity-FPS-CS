using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 武器类型定义
/// </summary>
public enum WeaponTypeEnum
{
    Rifle = 0,
    Pistol = 1,
    Kinfe = 3,
    ThrowObject = 4,
}

/// <summary>
/// 武器动作str定义
/// </summary>
public static class WeaponActionDefine
{
    public static readonly string None = "";
    public static readonly string Fire = "Fire";
    public static readonly string Reload = "Reload";
    public static readonly string LeftAttack = "LeftAttack";
    public static readonly string RightAttack = "RightAttack";
}

/// <summary>
/// 该动作是否可以被打断
/// </summary>
public static class WeaponActionBreakableDefine
{
    private static Dictionary<string, bool> dic;

    public static bool IsActionBreakable(string actionStr)
    {
        TryInit();
        bool isBreakable = true;
        dic.TryGetValue(actionStr, out isBreakable);
        return isBreakable;
    }

    private static void TryInit()
    {
        if (dic != null)
        {
            return;
        }
        dic = new Dictionary<string, bool>();
        dic.Add(WeaponActionDefine.None, true);
        dic.Add(WeaponActionDefine.Fire, false);
        dic.Add(WeaponActionDefine.Reload, false);
        dic.Add(WeaponActionDefine.LeftAttack, false);
        dic.Add(WeaponActionDefine.RightAttack, false);
    }
}

/// <summary>
/// 武器类型str定义
/// </summary>
public static class WeaponTypeStringDefine
{
    private static Dictionary<WeaponTypeEnum, string> dic;
    
    public static string GetStringByType(WeaponTypeEnum type)
    {
        InitDefine();
        string str = string.Empty;
        if (dic.TryGetValue(type, out str))
        {
            return str;
        }

        return str;
    }

    static void InitDefine()
    {
        if (dic != null)
            return;

        dic = new Dictionary<WeaponTypeEnum, string>();
        dic.Add(WeaponTypeEnum.Rifle, "Rifle");
        dic.Add(WeaponTypeEnum.Pistol, "Pistol");
    }
}

/// <summary>
/// 武器配置结构
/// </summary>
public class WeaponConfigData : ConfigBaseData
{
    public readonly uint weaponID;
    public readonly uint modelID;
    public readonly string dummyPointName;
    public readonly WeaponTypeEnum weaponType;
    public readonly string weaponName;
    public readonly uint clampCapacity;//弹夹容量（如果有的话
    public readonly uint maxCapacity;//最大子弹（如果有的话
    public readonly float animSpeed;//连射动画倍速
    public readonly string iconPath;//icon图标
    public readonly uint damage;// 伤害
    
    public WeaponConfigData(uint weaponID, uint modelID, string dummyPointName, WeaponTypeEnum type, string name, 
        uint clampCapactiy = 0, uint maxCapacity = 0, float animSpeed = 1, string iconPath = "", uint damage = 0)
    {
        this.weaponID = weaponID;
        this.modelID = modelID;
        this.dummyPointName = dummyPointName;
        this.weaponType = type;
        this.weaponName = name;
        this.clampCapacity = clampCapactiy;
        this.maxCapacity = maxCapacity;
        this.animSpeed = animSpeed;
        this.iconPath = iconPath;
        this.damage = damage;
    }
}


/// <summary>
/// 武器具体配置
/// </summary>
public class WeaponConfig : BaseConfig
{
    public WeaponConfig()
    {
        allCfgs.Add(new WeaponConfigData(1, 3, "RightHand", WeaponTypeEnum.Rifle,
            "AK-47", 30, 180, 3, "Texture/WeaponIcon/WeaponSpriteSheet2/AK", 30));
        
        allCfgs.Add(new WeaponConfigData(2, 4, "RightHand", WeaponTypeEnum.Pistol, 
            "Glock", 20, 80, 1.5f, "Texture/WeaponIcon/WeaponSpriteSheet2/Glock", 34));
    }
}
