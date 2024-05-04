using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

//gameLevelManager - 管理一场比赛:  开始进入地图  回合流程  结束退出地图
public class GameLevelManager : BaseManager
{
    public static GameLevelManager Instance;
    public List<GameLevelInstance> allLevelInstance;// svr data
    private GameLevelData tempLevelData;
    private GameLevelInstance tempLevelIns;
    
    private GameLevelInstance cachedLevelInstance;
    
    public GameLevelManager()
    {
        if (Instance == null)
            Instance = this;

        allLevelInstance = new List<GameLevelInstance>();
    }

    public override void Start()
    {
        Notifier.Regist(EventEnum.OnLevelFinish, EndLevel);
    }

    public override void OnDestroy()
    {
        Notifier.Regist(EventEnum.OnLevelFinish, EndLevel);
    }

    public void BeginLevel(MatchData matchData)
    {
        //关卡数据
        var levelData = new GameLevelData(matchData);
        var mapSceneName = levelData.GetMapSceneName();
        if (string.IsNullOrEmpty(mapSceneName))
            return;
        
        levelData.allPlayerDatas = GetAllPlayerData(levelData);
        tempLevelData = levelData;
        
        //开局数据
        LevelStartData startData = new LevelStartData();
        startData.allPlayerDatas = levelData.allPlayerDatas;
        startData.mapID = matchData.mapID;
        startData.beginRoundData = levelData.roundData;
        
        //开局
        CommonRpc.Instance.S2C_OnLevelCreatedRpc(startData);
        NetworkManager.Singleton.SceneManager.LoadScene(mapSceneName, LoadSceneMode.Single);
        levelData.state = GameLevelState.Loading;
    }

    public void EndLevel()
    {
        if(cachedLevelInstance == null)
            return;
        
        foreach (var aLevelInstance in allLevelInstance)
        {
            if (aLevelInstance == cachedLevelInstance)
            {
                allLevelInstance.Remove(aLevelInstance);
                break;
            }
        }
        
        cachedLevelInstance = null;
        tempLevelIns = null;
        tempLevelData = null;
        NetworkManager.Singleton.SceneManager.LoadScene("Login", LoadSceneMode.Single);
    }

    public void OnAllPlayerMapLoaded(SceneEvent sceneEvent)
    {
        if (sceneEvent.SceneName == "Login")
        {
            var allNetObjs = GameObject.FindObjectsOfType<NetworkObject>();
            if (allNetObjs != null)
            {
                foreach (var netObj in allNetObjs)
                {
                    if(!netObj || !netObj.gameObject)
                        continue;
                    
                    if(netObj.gameObject.isStatic)
                        continue;
                    if(netObj.IsSpawned)
                        netObj.Despawn();
                }
            }
        }
        
        if(allLevelInstance.Count == 0 || tempLevelData == null)
            return;
        tempLevelIns.LevelInit();
        tempLevelIns.LevelData.state = GameLevelState.Playing;
        tempLevelIns = null;
        tempLevelData = null;
        CommonRpc.Instance.S2C_OnLevelCreatedFinishRpc();
    }

    public void OnPlayerMapLoaded(SceneEvent sceneEvent)
    {
        if (tempLevelData == null)
            return;

        Debug.Log($"[GameLevelManager] OnPlayerMapLoaded scene:{sceneEvent.SceneName}, clientID:{sceneEvent.ClientId}");
        if (sceneEvent.ClientId == NetworkManager.ServerClientId)
        {
            //spawn关卡实例
            var go = Resources.Load<GameObject>(PathUtil.framworkPath + "GameLevelInstance");
            go = GameObject.Instantiate(go);
            GameLevelInstance levelInstance = go.GetComponent<GameLevelInstance>();
            levelInstance.LevelData = tempLevelData;
            tempLevelIns = levelInstance;
            allLevelInstance.Add(levelInstance);
            
            //同步实例
            var netObj = go.GetComponent<NetworkObject>();
            netObj.Spawn();
            return;
        }
        
        GameObject instance = GameObject.Instantiate(NetworkManager.Singleton.NetworkConfig.PlayerPrefab);
        var instanceNetworkObject = instance.GetComponent<NetworkObject>();
        instanceNetworkObject.SpawnWithOwnership(sceneEvent.ClientId, true);
        
        var pControl = instanceNetworkObject.GetComponent<PlayerController>();
        tempLevelIns.AppendPlayerController(pControl);
        pControl.RpPlayerData.Value = GetInLevelPlayerData(sceneEvent.ClientId);
    }
    
    public PlayerData GetInLevelPlayerData(ulong userID)
    {
        PlayerData data;
        foreach (var level in allLevelInstance)
        {
            if (level == null || level.LevelData.allPlayerDatas == null)
                continue;
            
            foreach (var playerData in level.LevelData.allPlayerDatas)
            {
                if (playerData.userData.userID == userID)
                    return playerData;
            }
        }

        Debug.LogError($"[GameLevelManager] GetInLevelPlayerData: userID:{userID} 不存在某个关卡实例中！");
        return default;
    }

    public GameLevelInstance GetGameLevelInstance()
    {
        if (cachedLevelInstance)
            return cachedLevelInstance;
        var levelInstance = GameObject.Find("GameLevelInstance(Clone)");
        if (!levelInstance)
            return null;
        cachedLevelInstance = levelInstance.GetComponent<GameLevelInstance>();
        return cachedLevelInstance;
    }
    
    private PlayerData[] GetAllPlayerData(GameLevelData levelData)
    {
        List<PlayerData> playerDatas = new List<PlayerData>();
        if (levelData == null)
            return playerDatas.ToArray();
        
        foreach (var pair in levelData.matchData.playersDic)
        {
            PlayerData data = LoadPlayerData(pair.Key);
            playerDatas.Add(data);
        }
        
        return playerDatas.ToArray();
    }
    
    private PlayerData LoadPlayerData(ulong uid)
    {
        PlayerData data = new PlayerData();
        data.userData.userID = uid;//理论上是不同的数值，但是这里没接数据库 所以就用和clientID一样了
        data.userData.userName = RandomUtil.GenerateRamdomStr();
        
        data.weaponData.rifleID = 1;
        data.weaponData.pistolID = 2;
        var cfg = (WeaponConfigData)ConfigManager.Instance.WeaponConfig.TryGetCfg(data.weaponData.rifleID);
        if (cfg != null)
        {
            data.weaponData.clampRifleLeftAmmo = cfg.clampCapacity;
            data.weaponData.curRifleAmmo = cfg.maxCapacity;
        }
        cfg = (WeaponConfigData)ConfigManager.Instance.WeaponConfig.TryGetCfg(data.weaponData.pistolID);
        if (cfg != null)
        {
            data.weaponData.clampPistolLeftAmmo = cfg.clampCapacity;
            data.weaponData.curPistolAmmo = cfg.maxCapacity;
        }
        data.weaponData.kinfeID = 0;
        data.weaponData.throwIDList = new uint[] { 0, 0, 0, 0 };
            
        data.settingData = new PlayerSettingData();
        data.settingData.mouseSensity = 70;
        data.settingData.windowSensity = 70;
            
        data.curWeaponID = data.weaponData.rifleID;
        data.modelID = 0;
        data.health = 100;
        return data;
    }
}