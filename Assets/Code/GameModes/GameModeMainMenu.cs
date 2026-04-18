using UnityEngine;

/// The main race mode game mode.
internal class GameModeMainMenu : GameMode
{
    // Menu prefab names
    private const string MENU_NAME_START_SCREEN = "MainMenuUI";
    private const string MENU_NAME_SELECT_CAR = "CarSelectUI";

    // all of this code sucsk but i have no time to make a better design
    internal enum MenuState
    { 
        None = 0,   
        StartScreen = 1,
        SelectCar = 2,
        StartRace = 3,
    };

    private MenuState _currentMenuState = MenuState.None; 

    private MenuState currentMenuState
    {
        get { return _currentMenuState; }   
        set
        {
            _currentMenuState = value;

            switch (_currentMenuState)
            {
                case MenuState.StartScreen:
                    UIManager.SetCurrentMenu(MENU_NAME_START_SCREEN);
                    break;
                case MenuState.SelectCar:
                    UIManager.SetCurrentMenu(MENU_NAME_SELECT_CAR);
                    break;
                case MenuState.StartRace:
                    GameManager.SetGameState(GameManager.GameModeEnum.RaceMode);
                    return;
            }

        }
    }


    internal override void OnEnter()
    {
        Debug.Log("Entering the main menu...");

        // ensure we are in the right scene
        if (GameManager.GetCurrentScene().name != GameManager.SCENE_MENU)
        {
            // blocks
            GameManager.SetCurrentScene(GameManager.SCENE_MENU);
        }
        
    }


    internal override void OnFrame()
    { 
    }

    internal override void OnFixedUpdate()
    {
        // hack for my BAD code design

        if (!UIManager.currentMenu)
        {
            // delibeerately structure like this to avoid endlessly calling getcurrentscene
            if (GameManager.GetCurrentScene().name == GameManager.SCENE_MENU)
                currentMenuState = MenuState.StartScreen;   
        }


        if (Input.GetKey(KeyCode.Return))
            currentMenuState++;
    }

    internal override void OnLeave()
    {

    }
}