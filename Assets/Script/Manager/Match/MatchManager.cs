using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.Netcode;
using UnityEngine;

public class MatchManager
{
    public static MatchManager Instance;
    public static Dictionary<GameMode, List<MatchData>> matchDic;//  gameMode -> matchList -> matchData
    
    public MatchManager()
    {
        if (null == Instance)
            Instance = this;

        matchDic = new Dictionary<GameMode, List<MatchData>>();
    }

    public void MatchGame(ulong clientID, GameMode gameMode, List<int> mapIDList)
    {
        Debug.Log($"[MatchManager] receive match, clientID={clientID}, gameMode={gameMode}, mapIDList={mapIDList}");
        List<MatchData> matchList;
        NetworkClient player = TryGetNetworkClient(clientID);
        if (player == null)
            return;
        
        if (gameMode != GameMode.Competitive)
        {
            Debug.LogError($"[MatchManager] unhandled gamemode={gameMode}");
        }
        
        //TODO 支持同时匹配多个MapID
        if (mapIDList == null || mapIDList.Count == 0)
        {
            Debug.LogError($"[MatchManager] 匹配失败!匹配数据中地图列表获取失败！ mapIDList is null or count=0!");
            return;
        }
        
        int mapID = mapIDList[0];
        if (matchDic.TryGetValue(gameMode, out matchList))
        {
            foreach (var match in matchList)
            {
                if (match.CanAppendPlayer())
                {
                    match.AppendClient(player);
                    return;
                }
            }
            MatchData matchData = new MatchData(gameMode, mapID);
            matchData.OnMatchSatisfyPlayer += OnMatchSatisfyPlayer;
            matchData.AppendClient(player);
            matchList.Add(matchData);
        }
        else
        {
            matchList = new List<MatchData>();
            var matchData = new MatchData(gameMode, mapID);
            matchData.OnMatchSatisfyPlayer += OnMatchSatisfyPlayer;
            matchData.AppendClient(player);
            matchList.Add(matchData);
            
            matchDic.Add(gameMode, matchList);
        }
    }

    private NetworkClient TryGetNetworkClient(ulong clientID)
    {
        NetworkClient player;
        NetworkManager.Singleton.ConnectedClients.TryGetValue(clientID, out player);
        if (player == null)
            Debug.LogError($"[MatchManager] clientID={clientID} 在已连接中的client中不存在！");
        return player;
    }

    private void OnMatchSatisfyPlayer(MatchData matchData)
    {
        GameLevelManager.Instance.BeginLevel(matchData);
    }

}
