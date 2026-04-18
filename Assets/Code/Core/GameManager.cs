using UnityEngine;
using UnityEngine.SceneManagement;
using System;

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
        RaceMode = 3,
        RaceFinished = 4,
    }

    // hacks so the gamemanager switches into the right state
    // i think in the future everything will have to be done in one scene
    internal const string SCENE_MENU = "MenuMain";
    internal const string SCENE_RACE_MODE = "Gameplay";
    internal const string SCENE_RACE_FINISHED = "PostRace";

    /// <summary>
    /// Maximum time an async operation can block (in ms).
    /// </summary>
    internal const int SCENE_LOAD_ASYNC_BLOCK_MAX_TIME = 30000;

    private static GameModeEnum state;

    internal static GameMode mode { get; private set; }

    // links the engine and gamemanager
    internal static GameManagerObject managerObject { get; private set; }

    private static Scene mainScene
    {
        get { return SceneManager.GetActiveScene(); }
    }

    private static bool initialised = false;

    /// <summary>
    /// hack - used for some debug features
    /// </summary>
    internal static bool additiveSceneIsUnloading = false; 

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

        GlobalSettings.Init();

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


    private static void OnRemoveSceneAdditiveDone(AsyncOperation operation)
    {
        additiveSceneIsUnloading = false; 
    }

    /// <summary>
    /// Unload an additive scene. THIS DOESN'T DO ANY SHIT!
    /// </summary>
    /// <param name="name">The additive scene to unload</param>
    internal static void RemoveSceneAdditive(string name)
    {
        // THIS IS A REALLY BAD THING BECAUSE WE DON'T DO ANYTHING TO WAIT FOR THE SCENE TO FINISH LOADING

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

        // no, you can't unload additive scenes async
        AsyncOperation async = SceneManager.UnloadSceneAsync(name);
        async.completed += OnRemoveSceneAdditiveDone;
        // allow it to block (our code is NOT set up to do anything else)
        additiveSceneIsUnloading = true; 

        return;
    }

    internal static void OnFrame()
    {
        if (Input.GetKey(KeyCode.R)
            && state == GameModeEnum.RaceMode
            && GlobalSettings.debugMode)
        {
            initialised = false; // make everything get reinit'ed
            SetGameState(GameModeEnum.Init);

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
                Debug.Log("GameManager::GetModeFromState selected invalid game state (Restarting the game...)");
                return new GameModeInit();
        }
     }
}
