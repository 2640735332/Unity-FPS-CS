using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 通用rpc定义
/// </summary>
public class CommonRpc : NetworkBehaviour
{
    public static CommonRpc Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        NetworkManager.OnConnectionEvent += OnConnectEvent;
        
        NetworkManager.SceneManager.OnSceneEvent += OnSceneEvent;
    }

    public override void OnNetworkDespawn()
    {
        NetworkManager.OnConnectionEvent -= OnConnectEvent;
        
        NetworkManager.SceneManager.OnSceneEvent -= OnSceneEvent;
    }
    
    #region SendToClient

    [Rpc(SendTo.ClientsAndHost)]
    public void S2C_OnLevelCreatedRpc(LevelStartData levelStartData)
    {
        Debug.Log("[CommonRPC]S2C_OnLevelCreatedRpc");
        Notifier.Dispatch(EventEnum.OnLevelStart, levelStartData);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void S2C_OnLevelCreatedFinishRpc()
    {
        Debug.Log("[CommonRPC]S2C_OnLevelCreatedFinishRpc");
        Notifier.Dispatch(EventEnum.OnLevelStartFinish);
    }
    
    #endregion
    

    #region SendToSvr

    [Rpc(SendTo.Server)]
    public void C2S_LoadSceneRpc(string sceneName)
    {
        if(string.IsNullOrEmpty(sceneName))
            return;
        
        NetworkManager.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    [Rpc(SendTo.Server)]
    public void C2S_BeginMatchRpc(ulong clientID, GameMode gameMode, int mapID)
    {
        var mapIDList = new List<int>();
        mapIDList.Add(mapID);
        MatchManager.Instance.MatchGame(clientID, gameMode, mapIDList);
    }

    [Rpc(SendTo.Server)]
    public void C2S_QuitLevelRpc(ulong clientID, GameMode gameMode)
    {
        
    }
    
    #endregion

    private void OnSceneEvent(SceneEvent sceneEvent)
    {
        switch (sceneEvent.SceneEventType)
        {
            case SceneEventType.Load:
                if (IsServer)
                    NetworkManager.SceneManager.LoadScene(sceneEvent.SceneName, sceneEvent.LoadSceneMode);
                break;
            case SceneEventType.Unload:
                // if(IsServer)
                //     NetworkManager.SceneManager.UnloadScene(sceneEvent.Scene);
                break;
            case SceneEventType.LoadComplete:
                if (IsClient)
                    UIManager.Instance.OpenUI(UIDefine.GetSceneDefView(sceneEvent.SceneName));
                else if(IsServer)
                    GameLevelManager.Instance.OnPlayerMapLoaded(sceneEvent);
                break;
            case SceneEventType.LoadEventCompleted:
                if(IsServer)
                    GameLevelManager.Instance.OnAllPlayerMapLoaded(sceneEvent);
                break;
            default:
                Debug.LogWarning($"[CommonRPC] OnSceneEvent, unhandled event type={sceneEvent.SceneEventType}");
                break;
        }
    }

    #region Server
    
    private void OnConnectEvent(NetworkManager mgr, ConnectionEventData eventData)
    {
        if (IsServer)
            Debug.Log($"[CommonRPC] svr OnConnectEvent eventType:{eventData.EventType}, clientID:{eventData.ClientId}");
        else
            Debug.Log($"[CommonRPC] client OnConnectEvent eventType:{eventData.EventType}, clientID:{eventData.ClientId}");
    }

    #endregion
    
    

}
