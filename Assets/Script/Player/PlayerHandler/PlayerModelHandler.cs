using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerModelHandler : PlayerBaseHandler
{
    private GameObject curPlayerModel;
    public GameObject CurPlayerModel => curPlayerModel;

    public ModelConfigData curPlayerModelCfg;
    public ModelConfigData curWeaponModelCfg;

    private GameObject curWeaponModel;
    public GameObject CurWeaponModel => curWeaponModel;

    public DummyPoint dummyPoint;

    public Dictionary<string, GameObject> modelDic;
    
    public PlayerModelHandler(PlayerController pControl) : base(pControl)
    {
        modelDic = new Dictionary<string, GameObject>();
    }

    public void RefreshModel(PlayerData playerData)
    {
        if (pControl.IsServer)
        {
            // load player model
            ModelConfigData playerModelCfg = (ModelConfigData)ConfigManager.Instance.ModelConfig.GetCfg(playerData.modelID);
            if (playerData.modelID == 0 || playerModelCfg == null)
            {
                Debug.Log($"[PlayerModelHandler] 显示玩家模型异常！playerData的modelID={playerData.modelID}, 配置={playerModelCfg}了");
                return;
            }

            if (curPlayerModelCfg == null || curPlayerModelCfg.modelID != playerModelCfg.modelID)
            {
                UnLoadModel(curPlayerModel);
                curPlayerModel = LoadModel(playerModelCfg.modelPath, pControl.transform);
                curPlayerModelCfg = playerModelCfg;
                dummyPoint = curPlayerModel.GetComponent<DummyPoint>();
            }
        }

        if (dummyPoint == null)
            dummyPoint = pControl.GetComponentInChildren<DummyPoint>();

        if (dummyPoint == null)
            return;
        
        dummyPoint.SetNeedHideObjects(!pControl.IsOwner);
        
        // load weapon model
        if (playerData.curWeaponID != 0)
        {
            WeaponConfigData weaponCfg = (WeaponConfigData)ConfigManager.Instance.WeaponConfig.GetCfg(playerData.curWeaponID);
            if (curWeaponModelCfg == null || curWeaponModelCfg.modelID != weaponCfg.modelID)
            {
                UnLoadModel(curWeaponModel, false);
                curWeaponModelCfg = (ModelConfigData)ConfigManager.Instance.ModelConfig.GetCfg(weaponCfg.modelID);
                var dummyTrans = dummyPoint.GetDummyTrans(curWeaponModelCfg.dummyPointName);
                curWeaponModel = LoadModel(curWeaponModelCfg.modelPath, dummyTrans, false);
                pControl.WeaponHandler.CurWeapon = curWeaponModel.GetComponent<Weapon>();
            }
        }
        else
        {
            UnLoadModel(curWeaponModel, false);
            curWeaponModel = null;
            curWeaponModelCfg = null;
            pControl.WeaponHandler.CurWeapon = null;
        }
    }

    public GameObject LoadModel(string modelPath, Transform parent, bool needSpawn = true)
    {
        GameObject go = null;
        if (modelDic.TryGetValue(modelPath, out go))
        {
            go.SetActive(true);
            var netObj = go.GetComponent<NetworkObject>();
            if (pControl.IsServer && needSpawn)
            {
                if(netObj != null && netObj.IsSpawned)
                    netObj.NetworkShow(pControl.OwnerClientId);
            }
        }
        else
        {
            go = Resources.Load<GameObject>(modelPath);
            go = GameObject.Instantiate(go, parent);

            var netObj = go.GetComponent<NetworkObject>();
            if (pControl.IsServer && needSpawn)
            {
                netObj.Spawn();
                netObj.TrySetParent(parent);
            }
            modelDic.Add(modelPath, go);
        }

        return go;
    }

    public void UnLoadModel(GameObject model, bool sync = true)
    {
        if (model == null)
            return;
        model.SetActive(false);
        if (sync)
        {
            var netObj = model.GetComponent<NetworkObject>();
            if (netObj != null && netObj.IsSpawned)
            {
                netObj.NetworkHide(pControl.OwnerClientId);
            }
        }
        
        //MonoBehaviour.Destroy(model);
    }

    public void ClearModel(string modelPath)
    {
        if (modelDic.ContainsKey(modelPath))
        {
            modelDic.Remove(modelPath);
        }
    }
}
