using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

/// <summary>
/// 用于创建具体的配置
/// </summary>
public class BaseConfig
{
    protected List<ConfigBaseData> allCfgs;

    public BaseConfig()
    {
        allCfgs = new List<ConfigBaseData>();
        allCfgs.Add(null);
    }
    
    public virtual ConfigBaseData GetCfg(uint key)
    {
        if (allCfgs == null || allCfgs.Count == 0 || allCfgs.Count < key)
        {
            Debug.LogError($"{ GetType().Name }: key={key}的配置不存在！");
            return default;
        }
        
        return allCfgs[(int)key];
    }

    public virtual ConfigBaseData TryGetCfg(uint key)
    {
        if (allCfgs == null || allCfgs.Count == 0 || allCfgs.Count <= key)
        {
            return default;
        }
        
        return allCfgs[(int)key];
    }

    public virtual ConfigBaseData GetCfg(int key1, int key2)
    {
        return default;
    }

    public virtual ConfigBaseData GetCfg(int key1, int key2, int key3)
    {
        return default;
    }
}
