
public static class ResoucesUtility
{
    public static string GetUIFormAsset(string assetName)
    {
        return string.Format("Prefabs/UI/Form/{0}", assetName);
    }

    public static string GetDataTableAsset(string assetName)
    {
        return string.Format("Datas/DataTable/{0}", assetName);
    }


    public static string GetPrefabAsset(string assetName)
    {
        return string.Format("Prefabs/{0}", assetName);
    }

    public static string GetAudioClipAsset(string assetName)
    {
        return string.Format("Audios/{0}", assetName);
    }

}