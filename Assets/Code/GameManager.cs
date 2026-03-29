using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

internal static class GameManager
{
    // hacks so the gamemanager switches into the right state
    // i think in the future everything will have to be done in one scene
    internal const string SCENE_MAIN_MENU = "MenuMain";
    internal const string SCENE_RACE = "Gameplay";
    internal const string SCENE_RACE_FINISHED = "PostRace";

    // The game state enum. Tells us what to do
    public enum GameModeEnum
    {
        Init = 0,
        Shutdown = 1,
        MainMenu = 2,
        BuildMode = 3,
        RaceMode = 4,
        RaceFinished = 5,
    }

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

    private static Scene scene
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

    public static void Start(GameManagerObject newManagerObject)
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

    public static GameModeEnum GetGameState()
    {
        return state;
    }

    public static void SetGameState(GameModeEnum newState)
    {
        state = newState;

        Debug.Log("Game state is changing to " + Enum.GetName(typeof(GameModeEnum), newState));

        // does a null check
        mode?.OnLeave();

        // get the new mode and enter it
        mode = GetModeFromState(newState);
        mode.OnEnter();
    }

    public static Scene GetCurrentScene()
    {
        return scene; 
    }

    public static void SetCurrentScene(string name)
    {
        // Sets scene field
        SceneManager.LoadScene(name);
    }

    public static void OnFrame()
    {
        if (Input.GetKey(KeyCode.R)
            && state == GameModeEnum.RaceMode)
        {
            initialised = false; // make everything get reinit'ed
            GameManager.SetCurrentScene(GameManager.SCENE_RACE);
            return;
        }

        mode.OnFrame();
    }

    public static void OnFixedUpdate()
    {
        mode.OnFixedUpdate();   
    }

    public static void OnLegacyGUI()
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
