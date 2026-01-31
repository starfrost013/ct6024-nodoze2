using UnityEngine;
using UnityEngine.SceneManagement;
using System;

internal static class GameManager
{
    // hacks so the gamemanager switches into the right state
    // i think in the future everything will have to be done in one scene
    internal const string SCENE_MAIN = "Gameplay";
    internal const string SCENE_RACE_FINISHED = "RaceDoneTemp";

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
        // this is all one scene
        scene = SceneManager.GetActiveScene();

        // don't bother changing the state on restart if the scene is not the main scene, since we already initialised
        if (scene.name != SCENE_MAIN)
            return;

        state = GameModeEnum.Init;

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
            case GameModeEnum.RaceFinished:
                return new GameModeRaceFinished();
            default:
                Debug.Log("GameManager::GetModeFromState selected invalid game state (Entering race mode...)");
                return new GameModeRaceMode();
        }
    
     }

}
