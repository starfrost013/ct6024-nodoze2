using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Runtime.CompilerServices;

internal static class GameManager
{
    // The game state enum. Tells us what to do
    public enum GameState
    {
        Init = 0,
        Shutdown = 1,
        Menu = 2,
        BuildMode = 3,
        RaceMode = 4,
        RaceFinished = 5,
    }

    private static GameState state;
    private static GameMode mode;

    private static Scene scene; 
    
    public static void Start()
    {
        state = GameState.Init;

        // this is all one scene
        scene = SceneManager.GetActiveScene();

        // temp
    }

    public static GameState GetGameState()
    {
        return state;
    }

    public static void SetGameState(GameState newState)
    {
        GameState oldState = state; 
        state = newState;
        OnGameStateChanged(newState);
    }

    public static Scene GetCurrentScene(Scene scene)
    {
        return scene; 
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
    private static GameMode GetModeFromState(GameState state)
    {
        switch (state)
        {
            case GameState.RaceMode:
                return new GameModeRaceMode();
            default:
                Debug.Log("GameManager::GetModeFromState selected invalid game state (Entering race mode...)");
                return new GameModeRaceMode();
        }
    
     }

    // privates
    private static void OnGameStateChanged(GameState newState)
    {
        Debug.Log("Game state is changing to " + Enum.GetName(typeof(GameState), newState));

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
