using UnityEngine;

// The main race mode game mode.
internal class GameModeInit : GameMode
{

    internal override void OnEnter()
    {
        Debug.Log("Initialising game");
        CarManager.Init();
        DominoManager.Init();

        GameManager.SetGameState(GameManager.GameModeEnum.RaceMode);
    }

    internal override void OnFrame()
    {
    }

    internal override void OnFixedUpdate()
    {

    }

    internal override void OnLeave()
    {

    }
}