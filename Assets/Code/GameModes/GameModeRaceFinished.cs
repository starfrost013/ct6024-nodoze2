using UnityEngine;
using UnityEngine.SceneManagement;

// The gamemode for when the race is finished
internal class GameModeRaceFinished : GameMode
{
    internal Timer restartTimer = new();

    private bool totalHack = false; 

    internal override void OnEnter()
    {
        // don't repeatedly reload
        if (GameManager.GetCurrentScene().name != GameManager.SCENE_RACE_FINISHED) 
            GameManager.SetCurrentScene(GameManager.SCENE_RACE_FINISHED);


        restartTimer.Start(10000);
    }

    internal override void OnFrame()
    {
        if (restartTimer.IsDone())
            GameManager.SetGameState(GameManager.GameModeEnum.RaceMode);
    }

    internal override void OnFixedUpdate()
    {

    }


    internal override void OnLeave()
    {
        /* This code is HORRIBLE but it is the only way I know to prevent a race condition at 2:30am that fucks everything up */

        GameManager.SetCurrentScene(GameManager.SCENE_MAIN);
    }
}