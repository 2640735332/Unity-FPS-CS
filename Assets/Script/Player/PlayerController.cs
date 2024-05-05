using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    private PlayerInputHandler inputHandler;// input
    private PlayerStateHandler stateHandler;// state
    
    private PlayerCharacHandler characHandler;// character
    private PlayerModelHandler modelHandler;// model
    private PlayerAnimHandler animHandler;// handleAnimation
    private PlayerWeaponHandler weaponHandler;// weapon
    private PlayerCameraHandler cameraHandler;// camera
    private PlayerPhysicHandler pyhsicHandler;// physic

    public PlayerCameraHandler CameraHandler => cameraHandler;
    public PlayerModelHandler ModelHandler => modelHandler;

    public PlayerWeaponHandler WeaponHandler => weaponHandler;

    public PlayerPhysicHandler PhysicHandler => pyhsicHandler;
    private PlayerStateParam tempStateParam;
    private NetworkVariable<PlayerStateParam> rpStateParam;
    private NetworkVariable<PlayerInputParam> rpInputParam;
    private NetworkVariable<PlayerData> rpPlayerData;

    public NetworkVariable<PlayerData> RpPlayerData
    {
        get { return rpPlayerData; }
        set { rpPlayerData = value; }
    }

    #region MonbehaviourOverride
    private void Awake()
    {
        rpInputParam = new NetworkVariable<PlayerInputParam>();
        rpStateParam = new NetworkVariable<PlayerStateParam>();
        rpPlayerData = new NetworkVariable<PlayerData>();
        
        var characterController = GetComponent<CharacterController>();
        characHandler = new PlayerCharacHandler(this, characterController);
        
        inputHandler = new PlayerInputHandler(this);
        stateHandler = new PlayerStateHandler(this);
        
        modelHandler = new PlayerModelHandler(this);
        
        animHandler = new PlayerAnimHandler(this);
        weaponHandler = new PlayerWeaponHandler(this);
        cameraHandler = new PlayerCameraHandler(this);
        pyhsicHandler = new PlayerPhysicHandler(this);
    }
    
    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            rpStateParam.OnValueChanged += OnStateParamChange;
            rpPlayerData.OnValueChanged += OnPlayerDataChange;
        }
        
        if (IsServer)
        {
            rpInputParam.Value = new PlayerInputParam();
            rpInputParam.Value.ResetAllParam();
        
            rpStateParam.Value = new PlayerStateParam();
            rpStateParam.Value.ResetAllState();
            rpPlayerData.Value = GameLevelManager.Instance.GetInLevelPlayerData(OwnerClientId);
        }
        
        if (!IsOwner)
        {
            var camGo = CameraHandler.PlayerCamera.gameObject;
            camGo.GetComponent<AudioListener>().enabled = false;
            CameraHandler.PlayerCamera.enabled = false;
        }
        
        modelHandler.RefreshModel(rpPlayerData.Value);
        if (IsOwner)
            Notifier.Dispatch(EventEnum.OnPlayerDataUpdate, rpPlayerData.Value);
    }


    private void Update()
    {
        if (IsClient && IsOwner)
        {
            var temp = inputHandler.GetPlayerInput();
            C2S_OnUpdateParamRpc(temp);
        }
    }
    private void FixedUpdate()
    {
        if (IsServer)
        {
            var actionStr = weaponHandler.GetActionStr(rpStateParam.Value);
            animHandler.HandlePlayerState(rpStateParam.Value, actionStr);
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            rpStateParam.OnValueChanged -= OnStateParamChange;
            rpPlayerData.OnValueChanged -= OnPlayerDataChange;
        }
    }

    #endregion
    
    private void OnStateParamChange(PlayerStateParam pre, PlayerStateParam cur)
    {
        modelHandler.RefreshModel(rpPlayerData.Value);
        // var actionStr = weaponHandler.GetActionStr(cur);
        // animHandler.HandlePlayerState(cur, actionStr);
    }

    private void OnPlayerDataChange(PlayerData pre, PlayerData cur)
    {
        Debug.Log("[PlayerController] OnPlayerDataChange");
        if (IsOwner)
        {
            Notifier.Dispatch(EventEnum.OnPlayerDataUpdate, cur);
        }
        modelHandler.RefreshModel(rpPlayerData.Value);
    }
    
    public void OnWeaponFire()
    {
        Debug.Log($"[PlayerWeaponHandler] weaponFire! weapon={weaponHandler.CurWeapon}");
        C2S_OnWeaponFireRpc();
    }

    public void OnWeaponReload()
    {
        Debug.Log($"[PlayerWeaponHandler] WeaponReload! weapon={weaponHandler.CurWeapon}");
        C2S_OnWeaponReloadRpc();
    }

    /// <summary>
    /// 重置玩家
    /// </summary>
    public void ResetController()
    {   
        characHandler.Reset();
        weaponHandler.Reset();
        animHandler.Reset();
        cameraHandler.Reset();
        pyhsicHandler.Reset();
        modelHandler.Reset();
    }
    
    #region NetworkFunc
    
    [Rpc(SendTo.Server)]
    private void C2S_OnUpdateParamRpc(PlayerInputParam inputParam)
    {
        if (IsInputLocked())
            return;
        
        tempStateParam.ResetAllState();
        tempStateParam.CurInputParam = inputParam;
        characHandler.HandlePlayerState(tempStateParam);
        cameraHandler.HandlePlayerState(tempStateParam);
        weaponHandler.HandlePlayerState(tempStateParam);
        modelHandler.RefreshModel(rpPlayerData.Value);
        rpStateParam.Value = tempStateParam; 
    }

    private bool IsInputLocked()
    {
        var playerDataLock = rpPlayerData.Value.IsLockInput();
        return playerDataLock;
    }

    public bool IsMoveAndFireLock()
    {
        var levelInstance = GameLevelManager.Instance.GetGameLevelInstance();
        if(!levelInstance)
            return false;
        return levelInstance.RoundData.roundState == LevelRoundState.Ready;
    }

    [Rpc(SendTo.Server)]
    private void C2S_OnWeaponFireRpc()
    {
        Debug.Log($"[PlayerController] C2S_OnWeaponFireRpc, source clientID={this.OwnerClientId}");
        weaponHandler.Fire();
    }
   
    [Rpc(SendTo.Server)]
    private void C2S_OnWeaponReloadRpc()
    {
        Debug.Log($"[PlayerController] C2S_OnWeaponReloadRpc, source clientID={this.OwnerClientId}");
        weaponHandler.Reload();
    }
    

    #endregion
    
}
