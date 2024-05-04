using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/*
 回合数据需要单独update  放在levelinstance中
 开局时需要下发回合数据到开局数据里  battleview打开时会去取
 后续更新走levelinstance
 */

/// <summary>
/// 局内回合数据，replicate
/// </summary>
public struct LevelRoundData : INetworkSerializable
{
    public LevelRoundState roundState;//回合状态
    public uint curRoundCount;//当前回合数
    public uint readyWaitTime;//准备等待时长
    public uint roundPlayTime;//回合游玩时长
    public uint boomWaitTime;//炸弹等待时长
    public uint switchNextRoundTime;//切换下一回合等待时长

    public uint leftCTCount;
    public uint leftTCount;

    public uint ctRound;
    public uint tRound;
    public float curLeftTime;//当前剩余时间
    public bool c4Planted;//当前回合C4是否安装下了

    public float curEndLeftTime;//end结算剩余时间

    public Action<LevelRoundState> OnSwitchStateFinish;//<newState>
    
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref roundState);
        serializer.SerializeValue(ref curRoundCount);
        serializer.SerializeValue(ref readyWaitTime);
        serializer.SerializeValue(ref roundPlayTime);
        serializer.SerializeValue(ref boomWaitTime);
        serializer.SerializeValue(ref switchNextRoundTime);
        serializer.SerializeValue(ref curLeftTime);
        serializer.SerializeValue(ref leftCTCount);
        serializer.SerializeValue(ref leftTCount);
        serializer.SerializeValue(ref ctRound);
        serializer.SerializeValue(ref tRound);
        serializer.SerializeValue(ref c4Planted);
    }

    /// <summary>
    /// 获取状态对应的固定时长
    /// </summary>
    /// <returns></returns>
    public uint GetStateFixedTime(LevelRoundState state)
    {
        switch (state)
        {
            case LevelRoundState.Ready:
                return readyWaitTime;
            case LevelRoundState.Play:
                return roundPlayTime;
            case LevelRoundState.End:
                return switchNextRoundTime;
            default:
                Debug.LogError($"[LevelRoundData] GetStateFixedTime state{state} is unhandled!");
                break;
        }

        return default;
    }

    /// <summary>
    /// 尝试切换到下一回合
    /// </summary>
    /// <returns></returns>
    public bool TrySwitchToNextState()
    {
        if (curLeftTime > 0)
            return false;
        
        return SwitchToNextState(roundState);
    }

    /// <summary>
    /// 切换到下一回合
    /// </summary>
    /// <param name="curState"></param>
    /// <returns></returns>
    public bool SwitchToNextState(LevelRoundState curState)
    {
        switch (curState)
        {
            case LevelRoundState.Ready:
                curLeftTime = roundPlayTime;
                roundState = LevelRoundState.Play;
                OnSwitchStateFinish?.Invoke(roundState);
                return true;
                
            case LevelRoundState.Play:
                roundState = LevelRoundState.End;
                curEndLeftTime = switchNextRoundTime;
                OnSwitchStateFinish?.Invoke(roundState);
                return true;
            case LevelRoundState.End:
                curLeftTime = readyWaitTime;
                roundState = LevelRoundState.Ready;
                curRoundCount++;
                OnSwitchStateFinish?.Invoke(roundState);
                return true;
            default:
                Debug.LogError($"[LevelRoundData] TrySwitchToNextState state{curState} is unhandled!");
                return false;
        }
    }

    public void UpdatePlayerCount(List<PlayerController> playerControllers)
    {
        leftTCount = 0;
        leftCTCount = 0;
        foreach (var pControl in playerControllers)
        {
            var data = pControl.RpPlayerData.Value;
            if (data.health > 0)
            {
                if (data.camp == GameCamp.T)
                    leftTCount++;
                else if (data.camp == GameCamp.CT)
                    leftCTCount++;
            }
        }
    }
}

/// <summary>
/// 回合状态
/// </summary>
public enum LevelRoundState
{
    Ready,//准备阶段
    Play,//游玩阶段
    End//回合结束阶段
}
