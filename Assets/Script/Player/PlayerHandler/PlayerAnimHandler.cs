using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnimHandler : PlayerBaseHandler
{
    public NetworkAnimator netAnimator;
    public Animator animator;

    private int xSpeedID;
    private int ySpeedID;

    private string curStateName;
    private string curActionName = "";
    private Coroutine stateCo;

    private Dictionary<string, AnimationClip> allClipInfo;
    
    public PlayerAnimHandler(PlayerController pControl) : base(pControl)
    {
        xSpeedID = Animator.StringToHash("XSpeed");
        ySpeedID = Animator.StringToHash("YSpeed");
    }

    public void HandlePlayerState(PlayerStateParam stateParam, string actionStr)
    {
        TryGetAnimator();
        if(animator == null)
            return;

        PlayerData playerData = pControl.RpPlayerData.Value;
        if (pControl.RpPlayerData.Value.health <= 0)
        { 
            if (curStateName != "Death")
            {
                animator.SetLayerWeight(animator.GetLayerIndex("Bottom"), 0f);
                animator.SetLayerWeight(animator.GetLayerIndex("Top"), 0f);
                animator.Play("Death");
                curStateName = "Death";
            }
            return;
        }
        animator.SetLayerWeight(animator.GetLayerIndex("Bottom"), 1f);
        animator.SetLayerWeight(animator.GetLayerIndex("Top"), 1f);
        
        var move = stateParam.move;
        animator.SetFloat(xSpeedID, move.x);
        animator.SetFloat(ySpeedID, move.z);
        
        if (actionStr == null || pControl.IsMoveAndFireLock())
            actionStr = string.Empty;
        
        if (playerData.curWeaponID == 0)
            return;
        
        var weaponCfg = (WeaponConfigData)ConfigManager.Instance.WeaponConfig.GetCfg(playerData.curWeaponID);
        if (weaponCfg == null)
            return;
        
        var weaponTypeStr = WeaponTypeStringDefine.GetStringByType(weaponCfg.weaponType);
        if (string.IsNullOrEmpty(weaponTypeStr))
        {
            Debug.LogError($"[PlayerAnimHandler] weaponID={playerData.curWeaponID}, weaponName={weaponCfg.weaponName}的weaponType在stringDefine中不存在定义！ ");
            return;
        }
        
        string stateName = string.Concat(weaponTypeStr, actionStr);
        animator.SetLayerWeight(1, 1);

        if (WeaponActionBreakableDefine.IsActionBreakable(curActionName) && stateCo != null)
        {
            pControl.StopCoroutine(stateCo);
            stateCo = null;
        }
        
        if (stateCo == null)
        {
            animator.Play(weaponTypeStr);
            animator.Play(stateName);
            curStateName = stateName;
            curActionName = actionStr;
            stateCo = pControl.StartCoroutine(OnAnimFinished(() =>
            {
                curStateName = string.Empty;
                curActionName = string.Empty;
                pControl.StopCoroutine(stateCo);
                stateCo = null;
            }));
        }
    }
    
    private IEnumerator OnAnimFinished(Action action)
    {
        if (string.IsNullOrEmpty(curStateName))
        {
            action();
        }
        else
        {
            AnimationClip clip = default;
            allClipInfo.TryGetValue(curStateName, out clip);
            if(clip != null)
                yield return new WaitForSeconds(clip.length);
            action();
        }
    }
    
    private void TryGetAnimator()
    {
        var playerGO = pControl.ModelHandler.CurPlayerModel;
        if (playerGO == null)
        {
            Debug.Log("[PlayerAnimHandler] TryGetAnimator playerGo is null");
            return;
        }
        
        if (netAnimator == null || animator == null)
        {
            netAnimator = playerGO.GetComponent<NetworkAnimator>();
            animator = playerGO.GetComponent<Animator>();

            allClipInfo = new Dictionary<string, AnimationClip>();
            var clipsInfo = animator.runtimeAnimatorController.animationClips;
            foreach (var clip in clipsInfo)
            {
                allClipInfo.Add(clip.name, clip);
            }
        }
        else
        {
            if (netAnimator.gameObject != playerGO || animator.gameObject != playerGO)
            {
                netAnimator = playerGO.GetComponent<NetworkAnimator>();
                animator = playerGO.GetComponent<Animator>();
            }
        }
    }

    public override void Reset()
    {
        curStateName = string.Empty;
    }
}
