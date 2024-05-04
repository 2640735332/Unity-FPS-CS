using System;
using TMPro;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class LoginView : UIBase {    // Start is called before the first frame update
    public Button svrListBtn;
    public Button exitGameBtn;
    
    private void Awake()
    {
        svrListBtn.onClick.AddListener(OnSvrListBtnClick);
        exitGameBtn.onClick.AddListener(OnExitGameBtnClick);
        Notifier.Regist(EventEnum.OnLevelStart, OnLevelStart);
    }
    private void OnDestroy()
    {
        svrListBtn.onClick.RemoveListener(OnSvrListBtnClick);
        exitGameBtn.onClick.RemoveListener(OnExitGameBtnClick);
        Notifier.UnRegist(EventEnum.OnLevelStart, OnLevelStart);
    }

    private void OnSvrListBtnClick()
    {
        if (NetworkManager.Singleton.IsClient)
        {
            CommonRpc.Instance.C2S_BeginMatchRpc(NetworkManager.Singleton.LocalClientId, GameMode.Competitive, 2);
            svrListBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Matching...";
            svrListBtn.interactable = false;
        }
    }
    
    
    private void OnLevelStart()
    {
        if (NetworkManager.Singleton.IsClient)
        {
            svrListBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Matching";
            svrListBtn.interactable = true;
        }
    }
    
    private void OnExitGameBtnClick()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
