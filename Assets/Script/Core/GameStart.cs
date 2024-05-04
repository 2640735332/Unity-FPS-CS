using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStart : MonoBehaviour
{
    [HideInInspector]
    public NetworkManager networkManager;
    public ConfigManager configManager;
    public ConnectManager connectManager;
    public GameLevelManager levelManager;
    public MatchManager matchManager;
    public ModelManager modelManager;

    public static GameStart Instance;

    private CommonRpc commonRpc;
    
    private GameObject uiRootPrefab;
    private GameObject networkManagerGo;

    public static long frameCount = 0;
    
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
        
        configManager = new ConfigManager();
        uiRootPrefab = Instantiate(Resources.Load<GameObject>(PathUtil.framworkPath + "UIRoot"), this.transform);
        UIManager.Instance.OpenUI(UIDefine.DebugView);
        
        var rpcGo = GameObject.Instantiate(Resources.Load<GameObject>(PathUtil.framworkPath + "CommonRpc"), this.transform);
        commonRpc = rpcGo.GetComponent<CommonRpc>();
        rpcGo.AddComponent<ModelManager>();
        
        networkManagerGo = Instantiate(Resources.Load<GameObject>(PathUtil.framworkPath + "NetworkManager"));
        networkManager = networkManagerGo.GetComponent<NetworkManager>();
        DontDestroyOnLoad(networkManagerGo);
        
        connectManager = new ConnectManager();
        levelManager = new GameLevelManager();
        matchManager = new MatchManager();
        
        levelManager.Awake();
        
        networkManager.ConnectionApprovalCallback += connectManager.OnCennectionApprovalCallback;
        networkManager.OnConnectionEvent += connectManager.OnConnectEvent;
    }

    private void Start()
    {
        levelManager.Start();
    }

    private void OnDestroy()
    {
        levelManager.OnDestroy();
    }

    private void Update()
    {
        frameCount++;
    }
}

/// <summary>
/// 游戏模式
/// </summary>
public enum GameMode
{
    Competitive = 1,//竞技模式
}

/// <summary>
/// 游戏阵营
/// </summary>
public enum GameCamp
{
    None = 0,//默认没有阵营
    CT = 1,//警察
    T = 2,//匪
}

/// <summary>
/// 游戏结束原因
/// </summary>
public enum GameFinishReason
{
    None = 0,//默认原因
    CTWin = 1,
    TWin = 2,
    Tie = 3,//平局
}
