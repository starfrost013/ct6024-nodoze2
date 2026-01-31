using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

// The gamemode for when the race is finished
internal class GameModeRaceFinished  : GameMode
{
    internal Timer restartTimer;

    internal override void OnEnter()
    {
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