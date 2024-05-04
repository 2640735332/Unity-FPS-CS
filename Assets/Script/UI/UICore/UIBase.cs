using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public enum UILayer
{
    None,
    Top,// top
    UI3D,// 3dui
    Normal,// normal
    Pop// pop
}

public class UIBase : MonoBehaviour
{
    [HideInInspector]
    public string uiName;
    
    [HideInInspector]
    public GameObject uiPrefab;
    
    public bool isFullScreen;
    public UILayer layer;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetValid()
    {
        if (this.uiPrefab == null)
            return;
        Transform transform = uiPrefab.GetComponent<Transform>();
        if (transform == null)
            return;
        
        transform.SetAsLastSibling();
        transform.gameObject.SetActive(true);
    }

    public void SetInValid()
    {
        if (this.uiPrefab == null)
            return;
        Transform transform = uiPrefab.GetComponent<Transform>();
        if (transform == null)
            return;
        
        transform.SetAsFirstSibling();
        transform.gameObject.SetActive(false);
    }

    protected void CloseSelf()
    {
        UIManager.Instance.CloseUI(this.uiName);
    }
}
