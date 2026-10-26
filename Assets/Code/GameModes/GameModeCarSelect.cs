using UnityEngine;

class GameModeCarSelect : GameMode
{
    private const string MENU_NAME_SELECT_CAR = "CarSelectUI";

    /// <summary>
    /// This is a stupid kludge to deal with the fact that the scene load overrides UIManager and deletes the UI as it loads before the scene
    /// </summary>
    private bool setUi = false; 

    internal override void OnEnter()
    {
        // ensure we are in the right scene
        if (GameManager.GetCurrentScene().name != GameManager.SCENE_MENU)
        {
            // blocks
            GameManager.SetCurrentScene(GameManager.SCENE_MENU);
        }

    }

    internal override void OnFrame() { }
    internal override void OnFixedUpdate() 
    { 
        if (!setUi)
        {
            UIManager.SetCurrentMenu(MENU_NAME_SELECT_CAR);
            setUi = true; 
        }

    }
    
    internal override void OnLeave() { } 
}