using UnityEngine;
using UnityEngine.SceneManagement;
using System;

internal class GameManager
{
    // The game state enum. Tells us what to do
    public enum GameState
    {
        Init = 0,
        Shutdown = 1,
        Menu = 2,
        BuildMode = 3,
        RaceMode = 4,
        UpgradeMode = 5,
    }

    private GameState state;
    private GameMode mode;

    private Scene scene; 
    
    public void Start()
    {
        state = GameState.Init;

        // this is all one scene
        scene = SceneManager.GetActiveScene();

        // temp
    }

    public void SetGameState(GameState newState)
    {
        GameState oldState = state; 
        state = newState;
        OnGameStateChanged(newState);
    }

    public void OnFrame()
    {
        mode.OnFrame();
    }

    // I don't like reflection, it cretaes large and ugly programs in C#
    private GameMode GetModeFromState(GameState state)
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
    private void OnGameStateChanged(GameState newState)
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
