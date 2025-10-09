using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AssetUtility
{
    public static string GetSceneAsset(string assetName)
    {
        return string.Format("Assets/Scenes/{0}.unity", assetName);
    }


}
