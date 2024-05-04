using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleView : UIBase {    // Start is called before the first frame update
    
    public Button backBtn;
    public TextMeshProUGUI healthText;
    public Image rifleImg;
    public TextMeshProUGUI rifleAmmoText;
    public Image pistolImg;
    public TextMeshProUGUI pistolAmmoText;
    public Image kinfeImg;
    public List<Image> throwsImg;

    public TextMeshProUGUI timeText;
    public TextMeshProUGUI ctNum;
    public TextMeshProUGUI tNum;

    public List<RawImage> ctImageList;
    public List<RawImage> tImageList;
    
    private PlayerData cachePlayerData;
    private LevelRoundData cacheRoundData;
    
    private void Awake()
    {
        Notifier.Regist(EventEnum.OnPlayerDataUpdate, OnPlayerDataUpdate);
        Notifier.Regist(EventEnum.OnLevelRoundUpdate, OnRoundDataUpdate);
        backBtn.onClick.AddListener(OnExitGameBtnClick);
    }

    private void OnEnable()
    {
        UpdateView();
    }


    private void OnDestroy()
    {
        backBtn.onClick.RemoveListener(OnExitGameBtnClick);
        Notifier.UnRegist(EventEnum.OnPlayerDataUpdate, OnPlayerDataUpdate);
        Notifier.UnRegist(EventEnum.OnLevelRoundUpdate, OnRoundDataUpdate);
    }
    
    private void OnPlayerDataUpdate(object data)
    {
        Debug.Log("[BattleView] OnPlayerDataUpdate");
        cachePlayerData = (PlayerData)data;
        UpdateView();
    }

    private void OnRoundDataUpdate(object data)
    {
        cacheRoundData = (LevelRoundData)data;
        UpdateRound();
    }
    
    public void UpdateView()
    {
        UpdatePlayer();
        UpdateRound();
    }

    public void UpdatePlayer()
    {
        healthText.text = cachePlayerData.health.ToString();
        var weaponData = cachePlayerData.weaponData;
        InternalSetSprite(rifleImg, weaponData.rifleID);
        InternalSetSprite(pistolImg, weaponData.pistolID);
        InternalSetSprite(kinfeImg, weaponData.kinfeID);

        rifleAmmoText.text = weaponData.clampRifleLeftAmmo + "/" + (weaponData.curRifleAmmo - weaponData.clampRifleLeftAmmo);
        pistolAmmoText.text = weaponData.clampPistolLeftAmmo + "/" + (weaponData.curPistolAmmo - weaponData.clampPistolLeftAmmo);
        
        for (int i = 0; i < throwsImg.Count; i++)
        {
            if (weaponData.throwIDList != null && weaponData.throwIDList.Length > i)
            {
                var throwID = weaponData.throwIDList[i];
                var showThrow = throwID != 0;
                
                var img = throwsImg[i];
                img.gameObject.SetActive(showThrow);
                if (showThrow)
                    InternalSetSprite(img, throwID);
            }
        }
    }

    public void UpdateRound()
    {
        float allSecs = 0;
        var state = cacheRoundData.roundState;
        if (state == LevelRoundState.Ready || state == LevelRoundState.Play)
        {
            allSecs = Mathf.Ceil(Mathf.Max(0, cacheRoundData.curLeftTime));
        }else if (state == LevelRoundState.End)
        {
            allSecs = Mathf.Ceil(Mathf.Max(0, cacheRoundData.curEndLeftTime));
        }
         
        int min = (int)(allSecs / 60);
        int sec = (int)allSecs % 60;
        timeText.text = min + ":" + sec;
            
        ctNum.text = cacheRoundData.ctRound.ToString();
        tNum.text = cacheRoundData.tRound.ToString();
    }

    private void InternalSetSprite(Image img, uint weaponID)
    {
        WeaponConfigData rifleCfg = (WeaponConfigData)ConfigManager.Instance.WeaponConfig.TryGetCfg(weaponID);
        if (rifleCfg != null && img != null)
            img.sprite = SpriteUtil.GetSprite(rifleCfg.iconPath);
        UpdateSelect(img, weaponID, cachePlayerData.curWeaponID);
    }

    private void UpdateSelect(Image img, uint weaponID, uint curWeaponID)
    {
        if (weaponID == curWeaponID)
            img.color = new Color(img.color.r, img.color.g, img.color.b, 1f);
        else
            img.color = new Color(img.color.r, img.color.g, img.color.b, 0.5f);
    }

    private void OnExitGameBtnClick()
    {
        UIManager.Instance.CloseUI(UIDefine.BattleView);
    }
}
