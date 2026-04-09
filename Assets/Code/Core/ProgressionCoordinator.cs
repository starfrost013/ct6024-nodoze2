
using NUnit.Framework;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditorInternal;
using UnityEngine;

/// <summary>
/// Game Progression Coordinator
/// 
/// Handles level transitions when you are in GameState::RaceMode
/// Might be required to move this into gamemanager or as a part of gamestateracemode
/// </summary>
internal static class ProgressionCoordinator
{
    //
    // STRUCTS
    //

    // these are referenced by Level%d basically
    internal class LevelReference
    { 
        internal string scene;
        internal string sceneOnNormalCompletion;
        internal string sceneOnSpecialCompletion;
    }

    //
    // FIELDS / PROPERTIES
    //

    private const string PROGRESSION_INFO_PATH = "Progression/ProgressionInfo";
    private const string NO_MORE_LEVELS = "End";
    internal static List<LevelReference> levels { get; private set; } = new(); 
    static TextAsset progressionInfo;

    private static LevelReference _currentLevel; 
    // avoid state duplication with this
    internal static LevelReference currentLevel
    {
        get
        {
            return _currentLevel;
        }

        private set
        {
            if (_currentLevel != null)
                GameManager.RemoveSceneAdditive(currentLevel.scene);    

            _currentLevel = value;

            // todo
            if (_currentLevel.scene == NO_MORE_LEVELS)
            {
                Debug.Log("You beat the game! [ADD END SCREEN OR GO BACK TO FIRST LEVEL OR TITLE SCREEN OR ADD RUN THING OR WHATEVER HERE]");
                return; 
            }

            GameManager.AddSceneAdditive(currentLevel.scene);
        }
    }

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
            Debug.LogError("**** NO progression data (SPECIFY LEVELS!) ****");
            return;
        }

        string currentScene = null, nextSceneNormal = null, nextSceneSpecial = null;

        // iterate through each level specified in the ini (sort by id so theo rder doesn't matter)
        for (int i = 0; i < numLevels; i++)
        {
            string levelString = "Level" + i;
            currentScene = ConfigParser.GetValue("Level" + i);
            nextSceneNormal = ConfigParser.GetValue("Level" + i + "CompleteNormal");
            nextSceneSpecial = ConfigParser.GetValue("Level" + i + "CompleteSpecial");
            
            // special warning is logged in different places since not all levels need multiple exits
            if (currentScene == null)
                Debug.LogWarning("Scene ID for level " + levelString + " (ID " + i + ") doesn't exist! Progression will be screwed up");
            else if (nextSceneNormal == null)
                Debug.LogWarning("Scene ID for level " + levelString + " (ID " + i + ") has no \"normal\" next level! Progression will be screwed up");
            else
            {
                Debug.Log("Level " + (i + 1) + " is specified to scene " + currentScene);

                levels.Add(new LevelReference
                {
                    scene = currentScene,
                    sceneOnNormalCompletion = nextSceneNormal,
                    sceneOnSpecialCompletion = nextSceneSpecial,  
                });
            }
        }

        Debug.Log("Progression coordinator initialised");
    }

    internal static LevelReference GetLevelByScene(string name)
    {
        foreach (LevelReference level in levels)
        {
            if (level.scene == name)
                return level;
        }

        return null;
    }

    internal static void SetLevel(string level)
    {
        currentLevel = GetLevelByScene(level);
    }

    internal static void AdvanceNormal()
    {
        if (string.IsNullOrWhiteSpace(currentLevel.sceneOnNormalCompletion))
        {
            Debug.LogError("ProgressionCoordinator failed to progress to the next \"NORMAL\" type level due to the lack of a OnNormalCompletion for the current level, " + currentLevel.scene);
            return;
        }

        if (levels.Count == 0)
            return;

        // first level
        currentLevel = (currentLevel == null) ? levels[0] : GetLevelByScene(currentLevel.sceneOnNormalCompletion);
    }

    internal static void AdvanceSpecial()
    {
        if (string.IsNullOrWhiteSpace(currentLevel.sceneOnSpecialCompletion))
        {
            Debug.LogError("ProgressionCoordinator failed to progress to the next \"SPECIAL\" type level due to the lack of a OnNormalCompletion for the current level, " + currentLevel.scene);
            return;
        }

        if (levels.Count == 0)
            return;

        // first level
        currentLevel = (currentLevel == null) ? levels[0] : GetLevelByScene(currentLevel.sceneOnSpecialCompletion);
    }
};