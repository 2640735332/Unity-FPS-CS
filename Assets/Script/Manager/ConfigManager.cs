using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigManager
{
    public static ConfigManager Instance;

    public ModelConfig ModelConfig;
    public WeaponConfig WeaponConfig;
    public GameModeConfig GameModeConfig;
    public MapConfig MapConfig;

    public ConfigManager()
    {
        if(Instance == null)
            Instance = this;

        ModelConfig = new ModelConfig();
        WeaponConfig = new WeaponConfig();
        GameModeConfig = new GameModeConfig();
        MapConfig = new MapConfig();
    }
}
