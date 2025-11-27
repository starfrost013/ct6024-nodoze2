using UnityEngine;
using System;

namespace Domino
{   
    // This is a static class that allows the loading and tracking of assets
    internal static class AssetManager
    {
        // todo: type determination stuff 
        // possibly have a list of assets here. but then unity could internally unload stuff and we'd have no way to tell 
        private static UInt32 numAssetsLoaded;

        internal static T LoadAsset<T>(string path) where T : UnityEngine.Object
        {
            // https://stackoverflow.com/questions/552629/c-sharp-print-the-class-name-from-within-a-static-function
            // Reflection is type safe, but 50 times slower!!!!
            Debug.Log("Game Asset Loader: Loading asset of type " + typeof(T).Name + "");

            T newAsset = Resources.Load<T>(path);

            if (!newAsset)
            {
                Debug.LogError("***** FAILED to load asset ******");
                return null; 
            }

            numAssetsLoaded++;

            return newAsset; 
        }

        internal static void UnloadAsset<T>(GameAsset asset) where T : GameAsset
        {
            Resources.UnloadAsset(asset);

            numAssetsLoaded--;
        }
    }
}
