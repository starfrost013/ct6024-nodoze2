using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

internal static class GameManager
{
    //
    // STRUCTS & ENUMS
    //

    // The game state enum. Tells us what to do
    internal enum GameModeEnum
    {
        Init = 0,
        Shutdown = 1,
        MainMenu = 2,
        BuildMode = 3,
        RaceMode = 4,
        RaceFinished = 5,
    }

    // hacks so the gamemanager switches into the right state
    // i think in the future everything will have to be done in one scene
    internal const string SCENE_MAIN_MENU = "MenuMain";
    internal const string SCENE_RACE_MODE = "Gameplay";
    internal const string SCENE_RACE_FINISHED = "PostRace";

    /// <summary>
    /// Maximum time an async operation can block (in ms).
    /// </summary>
    internal const int SCENE_LOAD_ASYNC_BLOCK_MAX_TIME = 30000;

    private static GameModeEnum state;

    private static GameMode _mode;

    internal static GameMode mode
    {
        get { return _mode; }
        private set { _mode = value; }
    }

    // links the engine and gamemanager
    private static GameManagerObject _managerObject; 

    internal static GameManagerObject managerObject
    {
        get { return _managerObject; }
        private set {  _managerObject = value; }
    }

    private static Scene mainScene
    {
        get { return SceneManager.GetActiveScene(); }
    }

    private static bool initialised = false;

    private static Player _player;

    internal static Player player
    {
        get { return _player; }
        private set { _player = value; }
    }

    internal static void Start(GameManagerObject newManagerObject)
    {
        // only initialise once
        if (initialised)
            return;

        // this is all one scene
        state = GameModeEnum.Init;
        managerObject = newManagerObject;
        player = new();

        SetGameState(state);

        initialised = true; 
        // temp
    }

    internal static GameModeEnum GetGameState()
    {
        return state;
    }

    internal static void SetGameState(GameModeEnum newState)
    {
        state = newState;

        Debug.Log("Game state is changing to " + Enum.GetName(typeof(GameModeEnum), newState));

        // does a null check
        mode?.OnLeave();

        // get the new mode and enter it
        mode = GetModeFromState(newState);
        mode.OnEnter();
    }

    //get rid of this

    internal static Scene GetCurrentScene()
    {
        return mainScene; 
    }

    internal static void SetCurrentScene(string name)
    {
        // Sets scene field
        SceneManager.LoadScene(name);
    }

    /// <summary>
    /// Load an additive scene.
    /// </summary>
    /// <param name="name">The additive scene to load.</param>
    internal static void AddSceneAdditive(string name)
    {
        SceneManager.LoadScene(name, LoadSceneMode.Additive);
    }

    /// <summary>
    /// Unload an additive scene. THIS DOESN'T DO ANY SHIT!
    /// </summary>
    /// <param name="name">The additive scene to unload</param>
    internal static void RemoveSceneAdditive(string name)
    {
        Scene sceneWeWant = SceneManager.GetSceneByName(name);  

        if (!sceneWeWant.IsValid())
        {
            Debug.LogError("GameManager::RemoveSceneAdditive - Please put the scene " + name + "in the build index!");
            return;
        }
        
        if (!sceneWeWant.isLoaded)
        {
            Debug.LogError("GameManager::RemoveSceneAdditive - the scene " + name + " is not even loaded so we would crash. THIS IS A BUG!");
            return;
        }

        // Our code sucks and isn't set up to do this, so, er, don't bother, and just block.
        // no, you can't unload additive scenes async
        AsyncOperation async = SceneManager.UnloadSceneAsync(name);

        // set up a timer
        Timer tmr = new();
        tmr.Start(SCENE_LOAD_ASYNC_BLOCK_MAX_TIME);

        while (!async.isDone)
        {
            if (tmr.GetElapsedTime() > SCENE_LOAD_ASYNC_BLOCK_MAX_TIME)
            {
                throw new TimeoutException("Asynchronous Scene Unload Operation for scene " + name + " took too long");
            }
        }
    }

    internal static void OnFrame()
    {
        if (Input.GetKey(KeyCode.R)
            && state == GameModeEnum.RaceMode)
        {
            initialised = false; // make everything get reinit'ed
            GameManager.SetCurrentScene(GameManager.SCENE_RACE_MODE);
            return;
        }

        mode.OnFrame();
    }

    internal static void OnFixedUpdate()
    {
        mode.OnFixedUpdate();   
    }

    internal static void OnLegacyGUI()
    {
        mode.OnLegacyGUI();
    }

    // I don't like reflection, it cretaes large and ugly programs in C#
    private static GameMode GetModeFromState(GameModeEnum state)
    {
        switch (state)
        {
            case GameModeEnum.Init:
                return new GameModeInit();
            case GameModeEnum.MainMenu:
                return new GameModeMainMenu();  
            case GameModeEnum.RaceMode:
                return new GameModeRaceMode();
            case GameModeEnum.RaceFinished:
                return new GameModeRaceFinished();
            default:
                Debug.Log("GameManager::GetModeFromState selected invalid game state (Entering race mode...)");
                return new GameModeRaceMode();
        }
     }
}
