
using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Game Progression Coordinator
/// 
/// Handles level transitions when you are in GameState::RaceMode
/// </summary>
internal static class ProgressionCoordinator
{
    //
    // STRUCTS
    //

    // these are referenced by Level%d basically
    internal struct LevelReference
    {
        internal string sceneId;
    }

    //
    // FIELDS / PROPERTIES
    //

    private const string PROGRESSION_INFO_PATH = "Progression/ProgressionInfo";

    internal static List<LevelReference> levels { get; private set; } = new(); 
    static TextAsset progressionInfo;

    //
    // METHODS
    //

    internal static void Init()
    {
        progressionInfo = AssetManager.LoadAsset<TextAsset>(PROGRESSION_INFO_PATH);
        ConfigParser.Parse(progressionInfo.text); // go

        if (!progressionInfo)
            Debug.LogError("ProgressionCoordinator::Init failed to load the progression info from " + PROGRESSION_INFO_PATH);

        bool result = int.TryParse(ConfigParser.GetValue("NumLevels"), out int numLevels);

        if (!result)
        {
            Debug.LogError("**** NO progression data (SPECIFY A NUMBER OF LEVELS!) ****");
            return;
        }

        string currentScene = null;

        // iterate through each level specified in the ini (sort by id so theo rder doesn't matter)
        for (int i = 0; i < numLevels; i++)
        {
            string levelString = "Level" + i;
            currentScene = ConfigParser.GetValue("Level" + i);

            if (currentScene == null)
                Debug.LogWarning("Scene ID for level " + levelString + " (ID " + i + ") doesn't exist! Progression will be screwed up");
            else
            {
                Debug.Log("Level " + (i + 1) + " is specified to scene " + currentScene);

                levels.Add(new LevelReference
                {
                    sceneId = currentScene,
                });
            }

        }

        Debug.Log("Progression coordinator initialised");
    }

    internal static void Advance()
    {

    }
};