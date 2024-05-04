using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteUtil
{
    private static Dictionary<string, Dictionary<string, Sprite>> loadedSprites = new Dictionary<string, Dictionary<string, Sprite>>();
    
    public static Sprite GetSprite(string spriteFullPath)
    {
        var arr = spriteFullPath.Split("/");
        if (arr == null || arr.Length == 0)
            return default;
        
        string spriteName = arr[arr.Length - 1];
        string texPath = spriteFullPath.Replace("/" + spriteName, "");
        if (loadedSprites.ContainsKey(texPath))
        {
            
            Dictionary<string, Sprite> spriteDic = loadedSprites[texPath];
            Sprite sprite;
            if (spriteDic.TryGetValue(spriteName, out sprite))
                return sprite;
            else
                Debug.LogError($"[SpriteUtill] sprite not exist! spirteName={spriteName}, texPath={texPath}, spriteFullName={spriteFullPath}");
        }
        else
        {
            Dictionary<string, Sprite> allSprite = new Dictionary<string, Sprite>();
            Sprite[] sprites = Resources.LoadAll<Sprite>(texPath);
            foreach (var sprite in sprites)
            {
                allSprite.Add(sprite.name, sprite);
            }
            loadedSprites.Add(texPath, allSprite);

            return GetSprite(spriteFullPath);
        }

        return default;
    }
}
