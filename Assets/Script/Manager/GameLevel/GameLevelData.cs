using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public enum GameLevelState
{
    Created,//刚创建出来，还未做初始化
    Loading,//loading进游戏
    Playing,//局内游玩
    Finish,//最后一回合结束，本局游戏结束状态
}

/*
    游戏逻辑

    开局机制：
       匹配成功人数达成，直接开局进入第1回合的准备阶段

    回合制
       共n回合
    回合配置
    回合流程
       准备阶段   x1秒
       战斗阶段    x2秒
       C4爆炸阶段   安装C4后 x3秒
       回合结束  死亡时间or玩家随意走动阶段  x4秒（不显示）

   结束机制：
       CT/T某一方达到胜利回合数 or 平局
       结束触发后，loading到主界面

   ds程序逻辑
       进入游戏：
       ds的匹配模块，满足匹配条件
       通知客户端匹配开始，下发匹配数据 - 客户端加载loading界面

       ds分配玩家到不同队伍中
       ds进行同步scene加载
       ds收到客户端scene加载完毕
       生成玩家数据，根据场景出生位配置spawn玩家

       ds收到所有客户端scene加载完毕
       同步完毕通知客户端匹配完成进入单局 - 客户端关闭loading界面

       开始回合：
       ds重置回合相关数据
       ds重置玩家数据、位置，同时随机挂载C4在某个T身上，关闭玩家移动&跳跃输入
       ds清空所有掉落物、道具效果、子弹痕迹
       ds开始准备倒计时
       ds准备倒计时结束，开始回合倒计时

       结束本回合：
       ds收到结束回合条件：某一阵营所有玩家全部优先死亡 || C4爆炸 || 回合时间结束 || C4被拆除
       ds开始游戏结束倒计时
       --进入下一回合，重新开始回合流程

       半场回合切换：
       ds在半场回合时，重置玩家阵营为地方阵营，之后所有回合重置时阵营位置不变

       结束游戏：
       ds在回合结束时判定是否满足游戏结束回合数：某一方回合数>total/2 || 双方回合数 =total/2
       ds关闭玩家所有输入
       ds通知双方玩家胜利or失败，并启动一个回主界面倒计时
       ds倒计时结束，同步scene场景到一个空场景并打开主界面

   client逻辑：
       loading界面
           取到开局数据
           显示loading界面
               loading进度条
               地图背景图片
               地图描述文字
           收到ds加载完毕，关闭loading界面

       战斗界面
            角色信息
                血量
                武器
                子弹
            
            回合信息
                敌我头像
                敌我回合数
                回合时间
                
            地图信息
                地图旋转
                我的位置
                我的视野范围
                我的视野范围内敌兵
                队友位置
                队友视野范围内敌兵
    */


/// <summary>
/// 一场比赛的数据
/// </summary>
public class GameLevelData
{
    public MatchData matchData;//匹配数据
    public GameLevelState state;
    public LevelRoundData roundData;//回合数据
    public PlayerData[] allPlayerDatas;
    private GameLevelInstance levelInstance;

    public GameLevelInstance LevelInstance
    {
        set
        {
            levelInstance = value;
        }
    }
    
    public GameLevelData(MatchData matchData)
    {
        this.matchData = matchData;
        this.state = GameLevelState.Created;
        
        roundData = new LevelRoundData();
        roundData.roundState = LevelRoundState.Ready;
        roundData.readyWaitTime = 15;
        roundData.curLeftTime = roundData.readyWaitTime;
        roundData.boomWaitTime = 40;
        roundData.roundPlayTime = 100;
        roundData.switchNextRoundTime = 8;
        roundData.curRoundCount = 1;
    }

    public string GetMapSceneName()
    {
        var mapCfg = (MapConfigData)ConfigManager.Instance.MapConfig.GetCfg((uint)matchData.mapID);
        if (mapCfg == null)
        {
            Debug.LogError($"[GameLevelData] GetMapSceneName, mapSceneName is empty! mapID{matchData.mapID}");
            return string.Empty;
        }
        return mapCfg.sceneName;
    }
}

/// <summary>
/// 开局数据
/// </summary>
public struct LevelStartData : INetworkSerializable
{
    public PlayerData[] allPlayerDatas;//所有玩家数据
    public LevelRoundData beginRoundData;//开局回合数据
    public int mapID;//地图ID
    
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref mapID);
        serializer.SerializeValue(ref allPlayerDatas);
    }
}
