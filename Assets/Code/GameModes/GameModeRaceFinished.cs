using UnityEngine;
using UnityEngine.SceneManagement;

/// The gamemode for when the race is finished
internal class GameModeRaceFinished : GameMode
{
    internal override void OnEnter()
    {
        // don't repeatedly reload
        if (GameManager.GetCurrentScene().name != GameManager.SCENE_RACE_FINISHED) 
            GameManager.SetCurrentScene(GameManager.SCENE_RACE_FINISHED);
    }

    internal override void OnFrame()
    {
        
    }

    internal override void OnFixedUpdate()
    {

    }

    internal override void OnLeave()
    {
        GameManager.SetCurrentScene(GameManager.SCENE_RACE_MODE);
    }
}