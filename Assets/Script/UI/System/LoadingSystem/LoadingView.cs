using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingView : UIBase {    
    
    public RawImage loadingTex;
    public TextMeshProUGUI loadingDesc;
    public Slider loadingSlider;


    private float overLoadTime = 180f;//超时时间
    private bool loadingFinish;
    private Coroutine co;
    
    private void Awake()
    {
        loadingFinish = false;
        co = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateView();
        
        Notifier.Regist(EventEnum.OnLevelStartFinish, OnLevelLoadingFinish);
    }

    private void UpdateView()
    {
        var levelStartData = ModelManager.Instance.LoadingModel.StartData;
        var mapID = levelStartData.mapID;
        MapConfigData mapCfg = (MapConfigData)ConfigManager.Instance.MapConfig.GetCfg((uint)mapID);
        if (mapCfg == null)
        {
            Debug.LogError($"[LoadingView] mapConfigData is null! mapID = {mapID}");
            return;
        }

        loadingTex.texture = Resources.Load<Texture>(mapCfg.mapPicturePath);
        loadingDesc.text = mapCfg.mapLoadingDesc;
        co = StartCoroutine(UpdateSlider());
    }

    private void OnLevelLoadingFinish()
    {
        this.loadingFinish = true;
    }
    
    private void OnDestroy()
    {
        if (co != null)
        {
            StopCoroutine(co);
            co = null;
        }
    }

    private IEnumerator UpdateSlider()
    {
        float minTime = 5f;
        float curTime = 0;
        float passedTime = 0f;

        while (!loadingFinish)
        {
            passedTime += Time.deltaTime;
            if (curTime <= 0.8 * minTime){
                curTime += Time.deltaTime;
                loadingSlider.value = curTime / minTime;
            }

            if (passedTime > this.overLoadTime)
                yield break;
            
            yield return Time.deltaTime;
        }

        if (curTime < 0.8 * minTime)
        {
            curTime = 0.8f * minTime;
            loadingSlider.value = curTime / minTime;
            yield return new WaitForSeconds(2f);
        }

        if (passedTime < overLoadTime)
        {
            loadingSlider.value = 1f;
            yield return new WaitForSeconds(0.7f);
            UIManager.Instance.OpenUI(UIDefine.BattleView);
            CloseSelf();
        }
        else
        {
            Debug.LogError("[LoadingView] loading time out!");   
        }
    }
    
}
