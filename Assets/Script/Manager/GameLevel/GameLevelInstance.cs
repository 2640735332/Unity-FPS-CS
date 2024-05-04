using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 关卡实例 保存关卡数据，处理关卡逻辑
/// 作为客户端与服务器关卡数据同步的桥梁
/// </summary>
public class GameLevelInstance : NetworkBehaviour
{
    private NetworkVariable<LevelRoundData> rpRoundData;//只用作同步 不保存最新数据
    private List<PlayerController> allPlayersControllers;
    private GameLevelData levelData;
    private LevelMapComponent levelMapComponent;

    public LevelRoundData RoundData => rpRoundData.Value;
    
    public GameLevelData LevelData
    {
        set
        {
            if (levelData != null)
                levelData.roundData.OnSwitchStateFinish -= OnRoundSwitchState;
            
            levelData = value;
            levelData.roundData.OnSwitchStateFinish += OnRoundSwitchState;
        }
        get
        {
            return levelData;
        }
    }

    private void Awake()
    {
        rpRoundData = new NetworkVariable<LevelRoundData>();
        allPlayersControllers = new List<PlayerController>();
    }
    
    private void Start()
    {
        
    }

    public override void OnNetworkSpawn()
    {
        if(IsClient)
            rpRoundData.OnValueChanged += OnRoundDataChange;

        if (IsServer)
        {
            var go = GameObject.Find(levelData.GetMapSceneName());
            levelMapComponent = go.GetComponent<LevelMapComponent>();
            Notifier.Regist(EventEnum.OnlySvr_OnPlayerDeath, OnPlayerDeath);
        }
    }

    public override void OnNetworkDespawn()
    {
        if(IsClient)
            rpRoundData.OnValueChanged -= OnRoundDataChange;

        if (IsServer)
        {
            Notifier.UnRegist(EventEnum.OnlySvr_OnPlayerDeath, OnPlayerDeath);
        }
    }

    private void FixedUpdate()
    {
        if (!IsServer)
            return;
        if (levelData.roundData.curEndLeftTime > 0)
            levelData.roundData.curEndLeftTime -= Time.deltaTime;
        CheckRoundStateChange();
        
        if (levelData.roundData.curLeftTime < 0)
            return;
        
        levelData.roundData.curLeftTime -= Time.deltaTime;
        if (rpRoundData.Value.curLeftTime - levelData.roundData.curLeftTime >= 1)
        {
            rpRoundData.Value = levelData.roundData;
        }
    }
    
    public void LevelInit()
    {
        if (allPlayersControllers == null || allPlayersControllers.Count == 0)
            return;
        
        GenerateAllPlayerCamp();
        GenerateAllPlayerSpawnPos();
        ResetAllPlayerToSpawnPos();
        var roundData = levelData.roundData;
        roundData.UpdatePlayerCount(allPlayersControllers);
        rpRoundData.Value = roundData;
    }

    private void OnSwitchToReady()
    {
        GenerateAllPlayerSpawnPos();
        ResetAllPlayerToSpawnPos();
        ResetAllPlayerData();
        foreach (var pControl in allPlayersControllers)
        {
            pControl.ResetController();
        }
        ResetMap();
        rpRoundData.Value = levelData.roundData;
    }

    private void OnSwitchToPlay()
    {
        ResetAllPlayerToSpawnPos();
        foreach (var pControl in allPlayersControllers)
        {
            pControl.ResetController();
        }
        ResetMap();
        rpRoundData.Value = levelData.roundData;
    }
    
    private void OnRoundDataChange(LevelRoundData pre, LevelRoundData cur)
    {
        Notifier.Dispatch(EventEnum.OnLevelRoundUpdate, cur);
    }
    
    private void OnPlayerDeath(object data)
    {
        PlayerData playerData = (PlayerData)data;
    }

    private void OnRoundSwitchState(LevelRoundState newState)
    {
        var levelFinishReason = IsLevelFinish();
        if (levelFinishReason != GameFinishReason.None)
        {
            if(levelData.roundData.roundState != LevelRoundState.End)
                rpRoundData.Value = levelData.roundData;
            levelData.state = GameLevelState.Finish;
            Notifier.Dispatch(EventEnum.OnLevelFinish, levelFinishReason);
            Debug.Log("[GameLevelInstance] OnRoundSwitchState:: GameLevelState.Finish");
        }
        else if(newState == LevelRoundState.Ready)
        {
            OnSwitchToReady();
        }else if (newState == LevelRoundState.Play)
        {
            OnSwitchToPlay();
        }
    }

    private void CheckRoundStateChange()
    {
        var roundData = levelData.roundData;
        var roundState = roundData.roundState;
        if (roundState == LevelRoundState.Ready)
        {
            levelData.roundData.TrySwitchToNextState();
            return;
        }

        if (roundState == LevelRoundState.Play)
        {
            var winCamp = IsAnyCampWin();
            if (winCamp != GameCamp.None)
            {
                if (winCamp == GameCamp.T)
                    levelData.roundData.tRound++;
                else
                    levelData.roundData.ctRound++;
                levelData.roundData.SwitchToNextState(LevelRoundState.Play);
                return;
            }
            levelData.roundData.TrySwitchToNextState();
        }

        if (roundState == LevelRoundState.End)
        {
            if (roundData.curEndLeftTime < 0)
                levelData.roundData.SwitchToNextState(LevelRoundState.End);
        }
    }

    private GameCamp IsAnyCampWin()
    {
        /*
        回合完成条件  
            安装了C4
                CT拆除了炸弹，CT胜利 or
                CT全部死亡，T胜利
            未安装C4
                play状态时间耗尽，CT胜利 or
                play状态某一方优先全部死亡，存活方胜利
                
        回合完成后
            roundState切换到 End
            roundState时间耗尽后，切换到下一回合 or 游戏结束
         */

        //安装了c4
        if (levelData.roundData.c4Planted)
        {
            //TODO 处理安装了C4的情况
            return GameCamp.None;
        }
        
        //未安装c4
        var allDeadCamp = IsAnyCampPlayerAllDead();
        if (allDeadCamp == GameCamp.None)
        {
            if (levelData.roundData.curLeftTime < 0.3f)
                return GameCamp.CT;
        }else if (allDeadCamp == GameCamp.T)
        {
            return GameCamp.CT;
        }else if (allDeadCamp == GameCamp.CT)
        {
            return GameCamp.T;
        }

        return GameCamp.None;
    }

    private GameCamp IsAnyCampPlayerAllDead()
    {
        bool ctAllDead = true;
        bool tAllDead = true;
        foreach (var pcontrol in allPlayersControllers)
        {
            var playerData = pcontrol.RpPlayerData.Value;
            if (playerData.health > 0)
            {
                if (playerData.camp == GameCamp.T)
                    tAllDead = false;
                else if (playerData.camp == GameCamp.CT)
                    ctAllDead = false;
            }
        }

        if (ctAllDead)
            return GameCamp.CT;
        else if (tAllDead)
            return GameCamp.T;
        else
            return GameCamp.None;
    }
    
    private GameFinishReason IsLevelFinish()
    {
        var totalRoundCount = levelData.matchData.ModeCfg.RoundCount;
        var roundData = levelData.roundData;
        var campWinRound = totalRoundCount / 2;
        var ctRound = roundData.ctRound;
        var tRound = roundData.tRound;

        if (ctRound == tRound && ctRound + tRound == totalRoundCount)
            return GameFinishReason.Tie;
        if (ctRound > campWinRound)
            return GameFinishReason.CTWin;
        if (tRound > campWinRound)
            return GameFinishReason.TWin;

        return GameFinishReason.None;
    }
    
    public Vector3 GetPlayerSpawnPos(PlayerController pControl, string sceneName)
    {
        var go = GameObject.Find(sceneName);
        var compo = go.GetComponent<LevelMapComponent>();
        var data = pControl.RpPlayerData.Value;
        GameObject pointGo = null;
        switch (data.camp)
        {
            case GameCamp.T:
                pointGo = compo.TSpawnPoints[(int)data.spawnPos];
                break;
            case GameCamp.CT:
                pointGo = compo.TSpawnPoints[(int)data.spawnPos];
                break;
            default:
                Debug.LogError($"[GameLevelManager]GetPlayerSpawnPos unhandled camp:{data.camp}");
                break;
        }

        return pointGo != null ? pointGo.transform.position : Vector3.zero;
    }
    
    public void AppendPlayerController(PlayerController pcontrol)
    {
        allPlayersControllers.Add(pcontrol);
    }
    
    //玩家阵营生成
    public void GenerateAllPlayerCamp()
    {
        for (int i = 0; i < allPlayersControllers.Count; i++)
        {
            var pControl = allPlayersControllers[i];
            var data = pControl.RpPlayerData.Value;
            data.camp = i % 2 == 0 ? GameCamp.CT : GameCamp.T;
            data.modelID = (uint)(i % 2 == 0 ? 1 : 2);
            pControl.RpPlayerData.Value = data;
        }
    }
    
    //玩家位置生成
    public void GenerateAllPlayerSpawnPos()
    {
        if (allPlayersControllers == null || allPlayersControllers.Count == 0)
            return;

        var ctPlyaerList = new List<PlayerController>();
        var tPlayerList = new List<PlayerController>();
        
        for (int i = 0; i < allPlayersControllers.Count; i++)
        {
            var pControl = allPlayersControllers[i];
            var data = pControl.RpPlayerData.Value;
            if(data.camp == GameCamp.T)
                tPlayerList.Add(pControl);
            else if(data.camp == GameCamp.CT)
                ctPlyaerList.Add(pControl);
            else
                Debug.LogError($"[GameLevelInstance] GenerateAllPlayerSpawnPos unhandled came{data.camp}");
        }

        var ctMaxPos = levelMapComponent.CTSpawnPoints.Count;
        var tMaxPos = levelMapComponent.TSpawnPoints.Count;
        GenerateCampPlayerSpawnPos(ctPlyaerList, ctMaxPos);
        GenerateCampPlayerSpawnPos(tPlayerList, tMaxPos);
    }

    private void GenerateCampPlayerSpawnPos(List<PlayerController> campPlayers, int maxPos)
    {
        var ctIdxList = new List<int>();
        for (int i = 0; i < maxPos; i++)
            ctIdxList.Add(i);
        
        for (int i = 0; i < campPlayers.Count; i++)
        {
            var pControl = allPlayersControllers[i];
            var data = pControl.RpPlayerData.Value;
            var idx = Random.Range(0, ctIdxList.Count);
            data.spawnPos = (uint)ctIdxList[idx];
            ctIdxList.RemoveAt(idx);
            pControl.RpPlayerData.Value = data;
        }
    }

    public void ResetAllPlayerToSpawnPos()
    {
        foreach (var pControl in allPlayersControllers)
        {
            var pData = pControl.RpPlayerData.Value;
            GameObject posGo = null;
            if (pData.camp == GameCamp.T)
                posGo = levelMapComponent.TSpawnPoints[(int)pData.spawnPos];
            else if (pData.camp == GameCamp.CT)
                posGo = levelMapComponent.CTSpawnPoints[(int)pData.spawnPos];

            if(!pControl)
                return;
            
            pControl.GetComponent<CharacterController>().enabled = false;
            pControl.transform.position = posGo.transform.position;
            pControl.transform.rotation = posGo.transform.rotation;
            pControl.GetComponent<CharacterController>().enabled = true;
            Debug.Log($"[GameLevelInstance] ResetAllPlayerToSpawnPos, player:{pControl.OwnerClientId}, newPos{posGo.transform.position}, playerNewPos{pControl.transform.position}");
        }
    }

    public void ResetAllPlayerData()
    {
        foreach (var pcontrol in allPlayersControllers)
        {
            var pData = pcontrol.RpPlayerData.Value;
            pData.health = 100;
            pcontrol.RpPlayerData.Value = pData;
        }
    }
    
    public void ResetMap()
    {
        ClearAllAmmo();
        ClearAllDrop();
        ClearlAllThrowEffects();
    }

    public void ClearAllAmmo()
    {
        
    }

    public void ClearAllDrop()
    {
        
    }

    public void ClearlAllThrowEffects()
    {
        
    }
}
