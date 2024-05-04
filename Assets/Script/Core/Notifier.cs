using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum EventEnum
{
    //player
    OnPlayerDataUpdate,//owner玩家数据更新
    
    //level
    OnLevelStart,//收到ds下发的开局
    OnLevelStartFinish,//收到ds开局完成
    OnLevelRoundUpdate,//回合数据变更
    OnlySvr_OnPlayerDeath,//玩家死亡
    OnLevelFinish,//游戏结束
    
    //round
}

public class Notifier
{
    private static Dictionary<EventEnum, Action<object>> eventOneParaDic = new Dictionary<EventEnum, Action<object>>();
    private static Dictionary<EventEnum, Action> eventDic = new Dictionary<EventEnum, Action>();
    
    public static void Dispatch(EventEnum eventEnum, object param = null)
    {
        if (eventDic.ContainsKey(eventEnum))
        {
            eventDic[eventEnum]?.Invoke();
        }
        
        if (eventOneParaDic.ContainsKey(eventEnum))
        {
            eventOneParaDic[eventEnum]?.Invoke(param);
        }
    }

    public static void Regist(EventEnum eventEnum, Action<object> callback)
    {
        if (eventOneParaDic.ContainsKey(eventEnum))
        {
            eventOneParaDic[eventEnum] += callback;
        }
        else
        {
            eventOneParaDic.Add(eventEnum, callback);
        }
    }

    public static void Regist(EventEnum eventEnum, Action callback)
    {
        if (eventDic.ContainsKey(eventEnum))
        {
            eventDic[eventEnum] += callback;
        }
        else
        {
            eventDic.Add(eventEnum, callback);
        }
    }
    
    public static void UnRegist(EventEnum eventEnum, Action<object> callback)
    {
        if (eventOneParaDic.ContainsKey(eventEnum))
        {
            eventOneParaDic[eventEnum] -= callback;
        }
    }
    
    public static void UnRegist(EventEnum eventEnum, Action callback)
    {
        if (eventDic.ContainsKey(eventEnum))
        {
            eventDic[eventEnum] -= callback;
        }
    }
}
