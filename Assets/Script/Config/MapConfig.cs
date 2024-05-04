using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapConfigData : ConfigBaseData
{
    public readonly int mapID;
    public readonly string mapName;
    public readonly string sceneName;
    public readonly string mapPicturePath;
    public readonly string mapLoadingDesc;

    public MapConfigData(int id, string name, string sceneName, string picPath, string mapLoadingDesc)
    {
        this.mapID = id;
        this.mapName = name;
        this.sceneName = sceneName;
        this.mapPicturePath = picPath;
        this.mapLoadingDesc = mapLoadingDesc;
    }
}


public class MapConfig : BaseConfig
{
    public MapConfig()
    {
        allCfgs.Add(new MapConfigData(1, "Cross Road", "TestScene", "Texture/Map/Dust2OverView", "Cross Road"));
        allCfgs.Add(new MapConfigData(2, "Defend Garage", "Level1", "Texture/Map/Level1OverView", "Defend Garage"));
    }
}
