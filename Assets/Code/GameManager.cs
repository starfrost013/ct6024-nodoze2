using UnityEngine;
using UnityEngine.SceneManagement;
using System;

internal static class GameManager
{
    // The game state enum. Tells us what to do
    public enum GameModeEnum
    {
        Init = 0,
        Shutdown = 1,
        Menu = 2,
        BuildMode = 3,
        RaceMode = 4,
        RaceFinished = 5,
    }

    private static GameModeEnum state;
    private static GameMode mode;
    // links the engine and gamemanager
    private static GameManagerObject managerObject; 

    private static Scene scene; 
    
    public static void Start(GameManagerObject newManagerObject)
    {
        state = GameModeEnum.Init;
        
        // this is all one scene
        scene = SceneManager.GetActiveScene();
        managerObject = newManagerObject;

        SetGameState(state);
        // temp
    }

    public static GameModeEnum GetGameState()
    {
        return state;
    }

    public static void SetGameState(GameModeEnum newState)
    {
        state = newState;
        OnGameStateChanged(newState);
    }

    public static Scene GetCurrentScene()
    {
        return scene; 
    }

    // Gets the game manager root object so that stuff can be instantiated
    public static GameManagerObject GetGameManagerObject()
    {
        return managerObject;
    }

    public static void OnFrame()
    {
        if (Input.GetKey(KeyCode.R))
            SceneManager.LoadScene(scene.buildIndex);
       
        mode.OnFrame();
    }

    public static void OnFixedUpdate()
    {
        mode.OnFixedUpdate();   
    }

    // I don't like reflection, it cretaes large and ugly programs in C#
    private static GameMode GetModeFromState(GameModeEnum state)
    {
        switch (state)
        {
            case GameModeEnum.Init:
                return new GameModeInit();
            case GameModeEnum.RaceMode:
                return new GameModeRaceMode();
            default:
                Debug.Log("GameManager::GetModeFromState selected invalid game state (Entering race mode...)");
                return new GameModeRaceMode();
        }
    
     }

    // privates
    private static void OnGameStateChanged(GameModeEnum newState)
    {
        Debug.Log("Game state is changing to " + Enum.GetName(typeof(GameModeEnum), newState));

        if (mode != null)
        {
            // leave the old mode
            mode.OnLeave();

        }

        // get the new mode and enter it
        mode = GetModeFromState(newState);
        mode.OnEnter();
    }

}
