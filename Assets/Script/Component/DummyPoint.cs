using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    dummyPointDefine:(string)
    
    LeftHand
    RightHand
  
 */

[Serializable]
public class DummyPoint : MonoBehaviour
{
    public List<GameObject> dummyPoints;

    public List<GameObject> needHideObjects;
    
    /// <summary>
    /// 获取dummy点Transform
    /// </summary>
    /// <param name="dummyName"></param>
    /// <returns></returns>
    public Transform GetDummyTrans(string dummyName)
    {
        if (dummyPoints == null || dummyPoints.Count <= 0)
            return default;
        foreach (var dummyGo in dummyPoints)
        {
            if (dummyGo.name.Equals(dummyName))
                return dummyGo.GetComponent<Transform>();
        }
        
        return default;
    }

    public void SetNeedHideObjects(bool needHide)
    {
        if (needHideObjects == null || needHideObjects.Count == 0)
            return;
        foreach (var go in needHideObjects)
        {
            go.SetActive(needHide);
        }
    }
}
