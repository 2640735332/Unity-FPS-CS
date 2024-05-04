using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private List<UIBase> uiStack;
    private Dictionary<string, UIBase> uiDic;// uinanme -> UIBase

    private static UIManager instance;
    public static UIManager Instance => instance;

    public Transform normalParent;
    public Transform popParent;
    public Transform ui3dParent;
    public Transform topParent;
    public Camera UICamera;

    void Awake()
    {
        if (instance == null)
            instance = this;
        
        if (uiStack == null)
        {
            uiStack = new List<UIBase>();
            uiDic = new Dictionary<string, UIBase>();
        }
    }

    public void OpenUI(string uiname)
    {
        if (uiStack == null)
        {
            Debug.LogError("UIManager: OpenUI, uistack == null!");
            return;
        }

        UIBase uibase = null;
        if (uiDic.TryGetValue(uiname, out uibase))
        {
            SetFullScreenUIInvalid();
            uibase.SetValid();
            PopToTop(uibase);
        }
        else
        {
            string fullPath = GetFullUIPath(uiname);
            GameObject go = Resources.Load<GameObject>(fullPath);
            if (go == null)
            {
                Debug.LogError($"UIManager: OpenUI, uiname is error!path = {fullPath}");
                return;
            }
            
            uibase = go.GetComponent<UIBase>();
            if (uibase == null)
            {
                Debug.LogError($"UIManager: OpenUI, ui没有添加UIBase组件！path = {fullPath}");
                return;
            }
            
            Transform parentTrans = this.GetUIParentTransform(uibase);
            go = Instantiate(go, parentTrans);
            uibase = go.GetComponent<UIBase>();
            
            uibase.uiName = uiname;
            uibase.uiPrefab = go;
            
            SetFullScreenUIInvalid();
            uibase.SetValid();
            uiDic.Add(uiname, uibase);
            uiStack.Insert(0, uibase);
        }
    }

    public void CloseUI(string uiname)
    {
        if(uiDic.ContainsKey(uiname))
            uiDic.Remove(uiname);

        UIBase uibase = null;
        for (int i = 0; i < uiStack.Count; i++)
        {
            if (uiStack[i].uiName == uiname)
            {
                uibase = uiStack[i];
                break;
            }
        }

        if (uibase != null)
        {
            if(uibase.isFullScreen)
                SetLastFullScreenUIValid();
            
            uiStack.Remove(uibase);
            if(uibase.uiPrefab) 
                Destroy(uibase.uiPrefab);
        }
    }
    
    private void PopToTop(UIBase uibase)
    {
        for (int i = 0; i < uiStack.Count; i++)
        {
            var curUIBase = uiStack[i];
            if (curUIBase.uiName == uibase.uiName)
            {
                var fist = uiStack[0];
                uiStack[0] = curUIBase;
                uiStack[i] = fist;
                break;
            }
        }
    }
    
    private bool IsUIExist(string uiname)
    {
        return uiDic.ContainsKey(uiname);
    }
    
    private string GetFullUIPath(string uiname)
    {
        return string.Concat(PathUtil.uipath, uiname);
    }

    private Transform GetUIParentTransform(UIBase uibase)
    {
        if (null == uibase)
            return this.normalParent;

        switch (uibase.layer)
        {
            case UILayer.None:
                return this.normalParent;
            case UILayer.Top:
                return this.topParent;
            case UILayer.Normal:
                return this.normalParent;
            case UILayer.Pop:
                return this.popParent;
            default:
                return this.normalParent;
        }
    }

    private void SetFullScreenUIInvalid()
    {
        foreach (var VARIABLE in uiStack)
        {
            if(VARIABLE.isFullScreen)
                VARIABLE.SetInValid();
        }
    }

    private void SetLastFullScreenUIValid()
    {
        for (int i = uiStack.Count - 1; i >= 0; i--)
        {
            var uibase = uiStack[i];
            if (uibase && uibase.isFullScreen)
            {
                uibase.SetValid();
            }
        }
    }
}
