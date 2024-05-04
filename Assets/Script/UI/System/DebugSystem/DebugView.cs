using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class DebugView : UIBase
{
    public Text envirText;
    public Button startClientBtn;
    public Button startSvrBtn;
    public GameObject bgGo;

    private string envir = string.Empty;
    string format = "yyyy-MM-dd HH:mm:ss";

    private void Awake()
    {
        var showStartBtn = envirText.text == string.Empty;
        startClientBtn.gameObject.SetActive(showStartBtn);
        startSvrBtn.gameObject.SetActive(showStartBtn);

        startSvrBtn.onClick.AddListener(OnStartSvrBtn);
        startClientBtn.onClick.AddListener(OnStartClientBtn);

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DateTime now = DateTime.Now;
        envirText.text = envir + "\n" + now.ToString(format) + "\nframe:" + GameStart.frameCount + "\nfps:" + Math.Ceiling(1 / Time.deltaTime);
    }

    private void OnDestroy()
    {
        startSvrBtn.onClick.RemoveListener(OnStartSvrBtn);
        startClientBtn.onClick.RemoveListener(OnStartClientBtn);
    }

    void OnStartClientBtn()
    {
        NetworkManager.Singleton.StartClient();
        envir = "Client";
        HideBtn();
        UIManager.Instance.OpenUI(UIDefine.LoginView);
    }

    void OnStartSvrBtn()
    {
        NetworkManager.Singleton.StartServer();
        envir = "Server";
        HideBtn();
    }

    void HideBtn()
    {
        startClientBtn.gameObject.SetActive(false);
        startSvrBtn.gameObject.SetActive(false);
        bgGo.SetActive(false);
    }
}
