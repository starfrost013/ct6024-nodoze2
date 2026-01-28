using UnityEngine;
using System;

// This is a static class that allows the loading and tracking of assets
internal static class AssetManager
{
    // todo: type determination stuff 
    // possibly have a list of assets here. but then unity could internally unload stuff and we'd have no way to tell 
    private static UInt32 numAssetsLoaded;

    internal static T LoadAsset<T>(string path, Type systemTypeInstance = null) where T : UnityEngine.Object
    {
        // https://stackoverflow.com/questions/552629/c-sharp-print-the-class-name-from-within-a-static-function
        // Reflection is type safe, but 50 times slower!!!!
        Debug.Log("Game Asset Loader: Loading asset of type " + typeof(T).Name + "" + " (path: " + path + ")");

        T newAsset = Resources.Load<T>(path);


        if (!newAsset)
        {
            Debug.LogError("***** FAILED to load asset: " + path + " ******");
            return null;
        }

        numAssetsLoaded++;

        return newAsset;
    }

    internal static void UnloadAsset<T>(T asset) where T : UnityEngine.Object
    {
        Resources.UnloadAsset(asset);

        numAssetsLoaded--;
    }
}