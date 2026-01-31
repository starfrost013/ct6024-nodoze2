using UnityEngine.SceneManagement;

// The gamemode for when the race is finished
internal class GameModeRaceFinished  : GameMode
{
    internal Timer restartTimer = new();

    internal override void OnEnter()
    {
        // don't repeatedly reload
        if (GameManager.GetCurrentScene().name != GameManager.SCENE_RACE_FINISHED) 
            SceneManager.LoadScene(GameManager.SCENE_RACE_FINISHED);

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
        SceneManager.LoadScene(GameManager.SCENE_MAIN);
    }
}