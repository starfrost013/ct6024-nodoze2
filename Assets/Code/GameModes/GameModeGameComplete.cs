using UnityEngine;

// The main race mode game mode.
internal class GameModeGameComplete : GameMode
{
    internal override void OnEnter()
    {
        if (GameManager.GetCurrentScene().name != GameManager.SCENE_GAME_COMPLETE)
            GameManager.SetCurrentScene(GameManager.SCENE_GAME_COMPLETE);
    }

    internal override void OnFixedUpdate()
    {
    }

    internal override void OnFrame()
    {
        
    }

    internal override void OnLeave()
    {
        // handled by ui
    }
}