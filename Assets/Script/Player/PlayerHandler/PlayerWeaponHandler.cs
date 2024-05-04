using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerWeaponHandler : PlayerBaseHandler
{
    private Weapon curWeapon;
    private Vector3 cameraCenter;

    private List<uint> tempWeaponIDList;
    
    public Weapon CurWeapon
    {
        get { return curWeapon; }
        set
        {
            curWeapon = value;
            curWeapon.Owner = this.pControl;
        }
    }
    
    public PlayerWeaponHandler(PlayerController pControl) : base(pControl)
    {
        
    }

    // playerInput + weaponCompo -> actionStr
    // playerInput + playerData -> playerInput + weaponCompo + weaponCfg -> typeStr + actionStr ->  anim
    public string GetActionStr(PlayerStateParam stateParam)
    {
        base.HandlePlayerState(stateParam);

        var curWeaponModel = pControl.ModelHandler.CurWeaponModel;
        if (curWeaponModel == null)
            return string.Empty;
        
        curWeapon = curWeaponModel.GetComponent<Weapon>();
        if (curWeapon == null)
        {
            Debug.LogError($"[PlayerWeaponHandler] weaponName={curWeaponModel.name}没有挂载weapon组件，这将使武器失效！, 请挂载weapon组件！");
            return string.Empty;
        }

        return curWeapon.GetActionStr(stateParam);
    }

    public void Fire()
    {
        if (curWeapon == null)
        {
            Debug.LogError("[WeaponHandler] curWeapon is null, but try to fire!");
            return;
        }

        if (!curWeapon.CanDoAttack())
            return;
        
        curWeapon.Attack();
        SyncWeaponData();
        
        cameraCenter.x = Screen.width / 2;
        cameraCenter.y = Screen.height / 2;
        var ray = pControl.CameraHandler.PlayerCamera.ScreenPointToRay(cameraCenter);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.CompareTag(TagDefine.Player))
            {
                var hitGo = hit.transform.gameObject;
                PlayerController pControl = hitGo.GetComponent<PlayerController>();
                Debug.Log($"[PlayerWeaponHandler] hit player! hit clientID= {pControl.OwnerClientId}");
                pControl.PhysicHandler.UnderAttack(curWeapon);
            }
        }
    }

    public void Reload()
    {
        if (curWeapon == null)
        {
            Debug.LogError("[PlayerWeaponHandler] curWeapon is null, but try to Reload!");
            return;
        }

        if (curWeapon is Gun)
        {
            var gun = curWeapon as Gun;
            gun.Reload();
            SyncWeaponData();
        }
        else
        {
            Debug.LogError("[PlayerWeaponHandler] curWeapon is not gun, but try to Reload!");
        }
    }

    private void SyncWeaponData()
    {
        var pData = pControl.RpPlayerData.Value;
        if (curWeapon.Type == WeaponTypeEnum.Rifle)
        {
            var rifle = curWeapon as Gun;
            pData.weaponData.curRifleAmmo = (uint)rifle.CurAmmo;
            pData.weaponData.clampRifleLeftAmmo = (uint)rifle.ClampLeftAmmo;
            pControl.RpPlayerData.Value = pData;
        }else if(curWeapon.Type == WeaponTypeEnum.Pistol)
        {
            var pistol = curWeapon as Gun;
            pData.weaponData.curPistolAmmo = (uint)pistol.CurAmmo;
            pData.weaponData.clampPistolLeftAmmo = (uint)pistol.ClampLeftAmmo;
            pControl.RpPlayerData.Value = pData;
        }
    }

    public override void HandlePlayerState(PlayerStateParam stateParam)
    {
        if (stateParam.scrollWheel == 0f)
            return;

        int moveCount = (int)(stateParam.scrollWheel / 0.1f);
        int weaponID = GetNextWeaponID(moveCount);
        if (weaponID == -1)
            return;

        PlayerData data = pControl.RpPlayerData.Value;
        data.curWeaponID = (uint)weaponID;
        pControl.RpPlayerData.Value = data;
    }

    private int GetNextWeaponID(int moveCount)
    {
        PlayerWeaponData pWeaponData = pControl.RpPlayerData.Value.weaponData;
        if (tempWeaponIDList == null)
            tempWeaponIDList = new List<uint>();
        else
            tempWeaponIDList.Clear();

        if (pWeaponData.rifleID != 0)
            tempWeaponIDList.Add(pWeaponData.rifleID);
        
        if(pWeaponData.pistolID != 0)
            tempWeaponIDList.Add(pWeaponData.pistolID);
        
        if(pWeaponData.kinfeID != 0)
            tempWeaponIDList.Add(pWeaponData.kinfeID);

        if (pWeaponData.throwIDList != null)
        {
            foreach (var throwID in pWeaponData.throwIDList)
            {
                if(throwID == 0)
                    continue;
                tempWeaponIDList.Add(throwID);
            }
        }
        
        if (tempWeaponIDList.Count == 0)
        {
            Debug.Log("[PlayerWeaponHandler] GetWeaponID: none weapon in player but try to change weapon!");
            return -1;
        }

        int curIdx = -1;
        for (int i = 0; i < tempWeaponIDList.Count; i++)
        {
            var weaponID = tempWeaponIDList[i];
            if(weaponID == 0 || weaponID != pControl.RpPlayerData.Value.curWeaponID)
                continue;

            curIdx = i;
            break;
        }

        if (curIdx == -1)
        {
            Debug.LogError($"[PlayerWeaponHandler] GetWeaponID: not found curIdx, curWeaponID={pControl.RpPlayerData.Value.curWeaponID}");
            curIdx = 0;
        }

        var count = tempWeaponIDList.Count;
        moveCount %= count;
        moveCount = (moveCount + count) % count;
        curIdx = (moveCount + curIdx) % count;
        return (int)tempWeaponIDList[curIdx];
    }
}
