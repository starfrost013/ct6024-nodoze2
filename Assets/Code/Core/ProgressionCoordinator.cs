using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Game Progression Coordinator
/// 
/// Handles level transitions when you are in GameState::RaceMode
/// Might be required to move this into gamemanager or as a part of gamestateracemode
/// 
/// Level progression information is loaded from the ProgressionInfo.txt file and a list of level references is obtained from this. These level references are then (on call from the current game mode)
/// obtained. "End" is used for the last level. When a level is switched to, if a level is already loaded its scene is unloadeda dditively and then
/// the scene for the level is loaded additively. Therefore, the Game Manager is no longer located in each level.
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

    /// <summary>
    /// If we need to re-enter normal progression, do this. Don't unload the old scene, even if it's set
    /// </summary>
    private static bool dontUnloadOldLevel;
   
    /// <summary>
    /// backing field for _currentlevel
    /// </summary>
    private static LevelReference _currentLevel; 
    // avoid state duplication with this

    /// <summary>
    /// The current level that is set.
    /// 
    /// NULL means that the game has not been satrted yet. I may change this later...
    /// </summary>
    internal static LevelReference currentLevel
    {
        get
        {
            return _currentLevel;
        }

        private set
        {
            // special case null
            if (value == null)
            {
                _currentLevel = value;
                return;
            }

            // special case "no more levels" (we'll probably change the handling for this) 

            // If progression was exited, we are assuming that a different scene was loaded since the game state changed. (check the old level for this)
            if ((_currentLevel != null) 
                
                && !dontUnloadOldLevel)
            {
                GameManager.RemoveSceneAdditive(_currentLevel.scene);
            }

            dontUnloadOldLevel = false;
            _currentLevel = value;

            Debug.Log("ProgressionCoordinator::CurrentLevel::set: Level is now " + _currentLevel.scene);
            GameManager.AddSceneAdditive(_currentLevel.scene);
        }
    }

    //
    // METHODS
    //

    /// <summary>
    /// Initialises the ProgressionCoordiantor.
    /// </summary>
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
                Debug.Log("Level " + (i + 1) + " is specified to scene " + currentScene + " normal exit = " + nextSceneNormal + ", special exit = " + nextSceneSpecial);

                levels.Add(new LevelReference
                {
                    scene = currentScene,
                    sceneOnNormalCompletion = nextSceneNormal,
                    sceneOnSpecialCompletion = nextSceneSpecial,  
                });
            }
        }

        // add a sentinel value
        // this level always sets itself over and over again
        levels.Add(new LevelReference
        { 
            scene = NO_MORE_LEVELS,
            sceneOnNormalCompletion = NO_MORE_LEVELS,
            sceneOnSpecialCompletion = NO_MORE_LEVELS,
        });

        currentLevel = null;


        Debug.Log("Progression coordinator initialised");
    }

    /// <summary>
    /// Get a level reference by its scene name.
    /// </summary>
    /// <param name="name">The name of the level to swtich to.</param>
    /// <returns>NULL is the level exists in the ProgressionInfo.txt file, otherwise NULL</returns>
    internal static LevelReference GetLevelByScene(string name)
    {
        foreach (LevelReference level in levels)
        {
            if (level.scene == name)
                return level;
        }

        return null;
    }

    /// <summary>
    /// Advance to the level set.
    /// </summary>
    /// <param name="scene">The scene of the level tos et to</param>
    internal static void SetLevel(string scene)
    {
        LevelReference reference = GetLevelByScene(scene);

        if (reference == null)
        {
            Debug.LogError("ProgressionCoordinator::SetLevel - failed, level " + scene + "does not exist!");
            return; 
        }

        currentLevel = reference;
    }

    /// <summary>
    /// Advance using a normal exit.
    /// </summary>
    internal static void AdvanceNormal()
    {
        // case: first level
        if (currentLevel == null)
            currentLevel = levels[0];
        else
        {
            if (string.IsNullOrWhiteSpace(currentLevel.sceneOnNormalCompletion))
            {
                Debug.LogError("ProgressionCoordinator failed to progress to the next \"NORMAL\" type level due to the lack of a OnNormalCompletion for the current level, " + currentLevel.scene);
                return;
            }

            currentLevel = GetLevelByScene(currentLevel.sceneOnNormalCompletion);
        }
    }

    /// <summary>
    /// Advance using a special exit.
    /// </summary>
    internal static void AdvanceSpecial()
    {
        if (currentLevel == null)
            currentLevel = levels[0];
        else
        {
            if (string.IsNullOrWhiteSpace(currentLevel.sceneOnSpecialCompletion))
            {
                Debug.LogError("ProgressionCoordinator failed to progress to the next \"SPECIAL\" type level due to the lack of a OnSpecialCompletion for the current level, " + currentLevel.scene);
                return;
            }

            currentLevel = GetLevelByScene(currentLevel.sceneOnSpecialCompletion);
        }

    }

    /// <summary>
    /// "Normal" progresson was exited (e.g. RaceFinished)
    /// 
    /// Whatever called this is going to replace the MainScene
    /// </summary>
    internal static void OnExitRaceScene()
    {
        dontUnloadOldLevel = true;
    }
};