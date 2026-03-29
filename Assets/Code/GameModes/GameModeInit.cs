using UnityEngine;

// The main race mode game mode.
internal class GameModeInit : GameMode
{

    internal override void OnEnter()
    {
        Debug.Log("Initialising game");

        GameManager.SetGameState(GameManager.GameModeEnum.MainMenu);
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