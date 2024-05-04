using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeConfigData : ConfigBaseData
{
    public GameMode GameMode;
    public readonly int PlayerNum;//开始游戏需要的玩家数量
    public readonly string Name;//玩法名称
    public readonly int RoundCount;//回合数量

    public GameModeConfigData(GameMode mode, int playerNum, string name, int roundCount)
    {
        this.GameMode = mode;
        this.PlayerNum = playerNum;
        this.Name = name;
        this.RoundCount = roundCount;
    }
}


public class GameModeConfig : BaseConfig
{
    public GameModeConfig()
    {
        allCfgs.Add(new GameModeConfigData(GameMode.Competitive, 2, "竞技模式", 2));
    }
}
