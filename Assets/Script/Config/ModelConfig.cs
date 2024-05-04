using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelConfigData : ConfigBaseData
{
    public readonly uint modelID = 0;
    public readonly string modelPath = "";
    public string dummyPointName;

    public ModelConfigData(uint modelID, string modelPath, string dummyPointName = "")
    {
        this.modelPath = modelPath;
        this.modelID = modelID;
        this.dummyPointName = dummyPointName;
    }
}

public class ModelConfig : BaseConfig
{
    private static readonly string characModelPath = "Prefab/Character/";
    private static readonly string weaponModelPath = "Prefab/Weapon/";
    private static readonly string rifleModelPath = "Prefab/Weapon/Rifle/";
    private static readonly string pistolModelPath = "Prefab/Weapon/Pistol/";
    
    public ModelConfig()
    {
        allCfgs.Add(new ModelConfigData(1, characModelPath + "CT01"));
        allCfgs.Add(new ModelConfigData(2, characModelPath + "T01"));
        allCfgs.Add(new ModelConfigData(3, rifleModelPath + "AK-47", "RightHand"));
        allCfgs.Add(new ModelConfigData(4, pistolModelPath + "Glock", "RightHand"));
    }
}
