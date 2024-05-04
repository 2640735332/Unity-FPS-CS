using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public enum MatchState
{
    Matching = 1,
    Playing = 2,
    Finish = 3,
}

public class MatchData
{
    private GameMode gameMode;
    private GameModeConfigData cfg;
    
    public GameMode GameMode => gameMode;
    public readonly int mapID;
    public readonly MatchState state;
    public readonly Dictionary<ulong, NetworkClient> playersDic;

    public GameModeConfigData ModeCfg => cfg;
    
    public Action<MatchData> OnMatchSatisfyPlayer;
    
    public MatchData(GameMode gameMode, int mapID)
    {
        this.gameMode = gameMode;
        playersDic = new Dictionary<ulong, NetworkClient>();
        cfg = (GameModeConfigData)ConfigManager.Instance.GameModeConfig.GetCfg((uint)gameMode);
        state = MatchState.Matching;
        this.mapID = mapID;
    }

    public void AppendClient(NetworkClient player)
    {
        if (null == player)
        {
            Debug.LogError("[MatchData] AppendClient player is null!");
            return;
        }

        var add = playersDic.TryAdd(player.ClientId, player);
        if(!add)
            Debug.LogError("[MatchData] AppendClient player is exist!");
        else
        {
            if(playersDic.Count == cfg.PlayerNum)
                OnMatchSatisfyPlayer?.Invoke(this);
        }
    }
    
    public bool CanAppendPlayer()
    {
        return playersDic.Count < cfg.PlayerNum && state == MatchState.Matching;
    }

    public void RemovePlayer(NetworkClient player)
    {
        if(null == player)
            return;
        
        if(playersDic.ContainsKey(player.ClientId))
            playersDic.Remove(player.ClientId);
    }

    public void ClearPlayers()
    {
        playersDic.Clear();
    }
    
}
