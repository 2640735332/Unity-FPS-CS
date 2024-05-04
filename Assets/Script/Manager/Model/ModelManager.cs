using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 数据model管理器
/// </summary>
public class ModelManager : MonoBehaviour
{
    private List<BaseModel> allModels;

    public LoadingModel LoadingModel;

    public static ModelManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        
        allModels = new List<BaseModel>();
        InitAllModels();

        foreach (var model in allModels)
        {
            model.Init();
        }

        foreach (var model in allModels)
        {
            model.RegistEvent();
        }
    }

    private void InitAllModels()
    {
        this.LoadingModel = new LoadingModel();
        allModels.Add(LoadingModel);
    }
    
    private void Update()
    {
        foreach (var model in allModels)
        {
            model.Tick();
        }
    }

    private void OnDestroy()
    {
        foreach (var model in allModels)
        {
            model.UnRegistEvent();
        }
        
        foreach (var model in allModels)
        {
            model.Destory();
        }
    }
}
