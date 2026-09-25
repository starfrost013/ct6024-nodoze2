//
// NODOZE Game
// Version 1.0 (CT5010): © 2025-2026 NotDominoes Team 
// Version 2.0 (CT6024): © 2025-2027 Connor Hyde (starfrost)
//

using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Unity.VisualScripting;

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
        GameComplete = 5,
    }

    // hacks so the gamemanager switches into the right state
    // i think in the future everything will have to be done in one scene
    internal const string SCENE_MENU = "MenuMain";
    internal const string SCENE_RACE_MODE = "Gameplay";
    internal const string SCENE_RACE_FINISHED = "PostRace";
    internal const string SCENE_GAME_COMPLETE = "GameComplete";

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

    private static Player _player;

    internal static Player player
    {
        get { return _player; }
        private set { _player = value; }
    }

    /// <summary>
    /// determines if the agme is pasued
    /// </summary>
    private static bool gamePaused = false;

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

        if (Input.GetKeyUp(KeyCode.Escape)
            && state > GameModeEnum.MainMenu)
        {
            if (!gamePaused)
            {
                UIManager.SetCurrentMenu("PauseUI");
                gamePaused = true;
            }
            else
            {
                UIManager.DestroyCurrentMenu();
                gamePaused = false; 
            }
        }

        if (!gamePaused)
            mode.OnFrame();
    }

    internal static void OnFixedUpdate()
    {
        if (!gamePaused)
            mode.OnFixedUpdate();   
    }

    internal static void OnLegacyGUI()
    {
        if (!gamePaused)
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
            case GameModeEnum.GameComplete:
                return new GameModeGameComplete();
            default:
                Debug.Log("GameManager::GetModeFromState selected invalid game state (Restarting the game...)");
                return new GameModeInit();
        }
     }
}
