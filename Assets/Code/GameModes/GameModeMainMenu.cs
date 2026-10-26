using UnityEngine;

/// The main race mode game mode.
internal class GameModeMainMenu : GameMode
{
    // Menu prefab names
    private const string MENU_NAME_START_SCREEN = "MainMenuUI";

    internal override void OnEnter()
    {
        Debug.Log("Entering the main menu...");

        UIManager.SetCurrentMenu(MENU_NAME_START_SCREEN);
    }

    internal override void OnFrame() { }

    internal override void OnFixedUpdate() { }

    internal override void OnLeave() { }
}