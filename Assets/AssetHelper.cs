using BTD_Mod_Helper.Api.Internal;
using UnityEngine;
using Object = Il2CppSystem.Object;

namespace XmasMod2025.Assets;

internal static class AssetHelper
{
    public static AssetBundle bundle;

    public static GameObject GetObject(string name)
    {
        return Get<GameObject>(name);
    }

    public static Sprite GetSprite(string name)
    {
        return Get<Sprite>(name);
    }

    public static void PrepareAssetBundle()
    {
        if (!bundle) bundle = ResourceHandler.Bundles["XmasMod2025-xmas"];
    }

    public static T Get<T>(string name) where T : Object
    {
        PrepareAssetBundle();
        return bundle.LoadAssetAsync<T>(name).asset.Cast<T>();
    }
}