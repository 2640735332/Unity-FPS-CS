using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingModel : BaseModel
{
    private LevelStartData startData;
    public LevelStartData StartData => startData;
    
    public override void RegistEvent()
    {
        Notifier.Regist(EventEnum.OnLevelStart, LevelStart);
        Notifier.Regist(EventEnum.OnLevelStartFinish, LevelStartFinish);
    }

    public override void UnRegistEvent()
    {
        Notifier.UnRegist(EventEnum.OnLevelStart, LevelStart);
        Notifier.UnRegist(EventEnum.OnLevelStartFinish, LevelStartFinish);
    }

    private void LevelStart(object curLevelAllPlayerData)
    {
        startData = (LevelStartData)curLevelAllPlayerData;
        UIManager.Instance.OpenUI(UIDefine.LoadingView);
    }

    private void LevelStartFinish()
    {
        
    }
}
