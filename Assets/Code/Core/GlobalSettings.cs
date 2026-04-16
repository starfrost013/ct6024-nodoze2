using System;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// GlobalSettings [April 16, 2026]
/// 
/// This static class is initialised, even before GameManager is put into STATE_INIT, to store configrable global sttae information for the build.
/// This will be loaded from the TextAssets.
/// </summary>
internal static class GlobalSettings
{
    /// <summary>
    /// Enables Connor's Super Debug Screen if enabled.
    /// </summary>
    internal static bool debugMode = false; 

    /// <summary>
    /// this is a big klutz so that i don't have to rewrite ConfigParser with 5 days before the demo
    /// </summary>
    private static string settingsFileText;

    internal static void Init()
    {
        Debug.Log("Loading global settings...");
        // This is a bit special. Everything else is put in resources.assets, but we use streamingassets since we want the user to be able to modify this file.
        // since we are not using TextAsset we don't need to use a .txt extension. Woo!
        string settingsPath = Path.Combine(Application.streamingAssetsPath, "GameConfig.cfg");

        try
        {
            settingsFileText = File.ReadAllText(settingsPath); // not too big, not a problem
        }
        catch (Exception exception)
        {
            Debug.LogWarning("Dammit!! Couldn't load GameConfig.cfg. The error was:\n\n " + exception);
        }

        // throw it in
        ConfigParser.Parse(settingsFileText);

        // we don't really care if these exist
        bool.TryParse(ConfigParser.GetValue("DebugMode"), out debugMode);
    }
};
