using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITest
{
    public static void OpenLoginUI()
    {
        UIManager.Instance.OpenUI(UIDefine.LoginView);
    }
}
