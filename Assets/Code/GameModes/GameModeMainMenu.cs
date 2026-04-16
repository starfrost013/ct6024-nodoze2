using UnityEngine;

/// The main race mode game mode.
internal class GameModeMainMenu : GameMode
{

    internal override void OnEnter()
    {
        Debug.Log("Entering the main menu...");

        // ensure we are in the right scene
        if (GameManager.GetCurrentScene().name != GameManager.SCENE_MAIN_MENU)
        {
            // blocks
            GameManager.SetCurrentScene(GameManager.SCENE_MAIN_MENU);
        }
    }

    internal override void OnFrame()
    { 
    }

    internal override void OnFixedUpdate()
    {
        if (Input.GetKey(KeyCode.Return))
        {
            // RaceMode will set up the scene, etc
            GameManager.SetGameState(GameManager.GameModeEnum.RaceMode);    
        }
    }

    internal override void OnLeave()
    {

    }
}