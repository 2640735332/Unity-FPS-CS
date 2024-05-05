using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

public class DebugView : UIBase
{
    public Text envirText;
    public Button startClientBtn;
    public Button startSvrBtn;
    public GameObject bgGo;
    public InputField ipInputField;
    public InputField portInputField;

    private const string ip = "ip";
    private const string port = "port";
    private const string defIP = "127.0.0.1";
    private const string defPort = "7777";
    
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
        ipInputField.text = PlayerPrefs.GetString(ip, defIP);
        portInputField.text = PlayerPrefs.GetString(port, defPort);
    }

    // Update is called once per frame
    void Update()
    {
        DateTime now = DateTime.Now;
        envirText.text = envir + "\n" + now.ToString(format) + "\nframe:" + GameStart.frameCount + "\nfps:" + Math.Ceiling(1 / Time.deltaTime) 
                         + "\n ip:" + PlayerPrefs.GetString(ip, defIP)+":"+PlayerPrefs.GetString(port, defPort);;
    }

    private void OnDestroy()
    {
        startSvrBtn.onClick.RemoveListener(OnStartSvrBtn);
        startClientBtn.onClick.RemoveListener(OnStartClientBtn);
    }

    void OnStartClientBtn()
    {
        var utp = (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
        utp.SetConnectionData(ipInputField.text, ushort.Parse(portInputField.text));
        
        var res = NetworkManager.Singleton.StartClient();
        if (!res)
        {
            Debug.LogError($"[DebugView] StartClient failed!");
            return;
        }
        
        envir = "Client";
        HideBtn();
        UIManager.Instance.OpenUI(UIDefine.LoginView);
    }

    void OnStartSvrBtn()
    {
        var utp = (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
        utp.SetConnectionData(ipInputField.text, ushort.Parse(portInputField.text));
        var res = NetworkManager.Singleton.StartServer();
        if (!res)
        {
            Debug.LogError("[DebugView] StartServer failed!");
            return;
        }
        
        envir = "Server";
        HideBtn();
    }

    void HideBtn()
    {
        PlayerPrefs.SetString(ip, ipInputField.text);
        PlayerPrefs.SetString(port, portInputField.text);
        startClientBtn.gameObject.SetActive(false);
        startSvrBtn.gameObject.SetActive(false);
        bgGo.SetActive(false);
        ipInputField.gameObject.SetActive(false);
        portInputField.gameObject.SetActive(false);
    }
}
