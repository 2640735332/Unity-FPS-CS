using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     LoginUI
///     SerevrListUI
///     LoadingUI
///     BattleUI
///     GunStoreUI
/// </summary>
public class UIDefine
{
    public static readonly string LoginView = "LoginView";
    public static readonly string BattleView = "BattleView";
    public static readonly string DebugView = "DebugView";
    public static readonly string LoadingView = "LoadingView";

    private static Dictionary<string, string> sceneDefView;

    public static string GetSceneDefView(string sceneName)
    {
        if (sceneDefView == null)
        {
            sceneDefView = new Dictionary<string, string>();
            sceneDefView.Add("TestScene", BattleView);
            sceneDefView.Add("Level1", BattleView);
            sceneDefView.Add("Login", LoginView);
        }

        string res;
        sceneDefView.TryGetValue(sceneName, out res);
        return res;
    }
}
