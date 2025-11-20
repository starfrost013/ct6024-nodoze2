using UnityEngine;

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


    public void SetGameState(GameState newState)
    {
        GameState oldState = state; 
        state = newState;
        OnGameStateChanged(newState);
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
        // leave the old mode
        mode.OnLeave();

        // get the new mode and enter it
        mode = GetModeFromState(newState);
        mode.OnEnter();
    }

    internal void OnFrame()
    {
        mode.OnFrame();
    }
}
