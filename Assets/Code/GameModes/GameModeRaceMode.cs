using System;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.Rendering.DebugUI.MessageBox;

// The main race mode game mode/.
internal class GameModeRaceMode : GameMode
{
    /* At some point we need to put all this in a config file */
    const int RACE_START_TIME = 3000;

    internal enum RaceState
    {
        Starting = 0,
        Active = 1,
        Finished = 2,
    };

    // externally accessed property -- the subset of the race
    internal RaceState raceState { get; private set; }

    Timer raceStartTimer = new();
    Timer gameTimer = new();

    internal string raceConfigFile;

    internal override void OnEnter()
    {
        Debug.Log("Entering race...");
        raceState = RaceState.Starting;

        CarManager.Init();
        DominoManager.Init();

        // wrost hack ever
        if (raceStartTimer.HasStarted())
            raceStartTimer.Start(Timer.TIMER_CONTINUE_FOREVER);
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

    private void DrawTimer(GUIStyle raceGuiStyle)
    {
        if (!gameTimer.HasStarted())
            gameTimer.Start(Timer.TIMER_CONTINUE_FOREVER);

        Int64 totalTime = gameTimer.GetElapsedTime();

        // easier to use constants. 60000 seconds 
        Int64 milliseconds = totalTime % 1000;
        Int64 seconds = (totalTime / 1000) % 60;
        Int64 minutes = ((totalTime / 1000) / 60) % 60;

        string millisecondsString = milliseconds.ToString(), secondsString = seconds.ToString(), minutesString = minutes.ToString();

        // this might be a slow operation
        if (milliseconds < 10)
            millisecondsString = "00" + millisecondsString;
        else if (milliseconds < 100)
            millisecondsString = "0" + millisecondsString;

        if (seconds < 10)
            secondsString = '0' + secondsString;

        if (minutes < 10)
            minutesString = '0' + minutesString;

        raceGuiStyle.fontSize = 36;

        string timerString = minutesString + ":" + secondsString + "." + millisecondsString;
        GUI.color = Color.red;
        GUI.Label(new Rect(Screen.width - 170, 0, 400, 100), timerString, raceGuiStyle);
    }

    private void DrawMoneyAmount(GUIStyle raceGuiStyle)
    {
        Car car = GameManager.player.car;
        GUI.color = Color.green;

        float fuelPercentage = (GameManager.player.stats.money) * 100;

        float x = Screen.width - 205;
        float y = Screen.height - 90;

        GUI.Label(new Rect(x, y, 400, 100), "Money: $" + GameManager.player.stats.money, raceGuiStyle);

    }

    private void DrawFuelGauge(GUIStyle raceGuiStyle)
    {
        Car car = GameManager.player.car;  
        GUI.color = Color.blue;

        float fuelPercentage = (car.physics.fuelCurrent / car.GetCarModifiers().fuelMax) * 100;

        float x = Screen.width - 205;
        float y = Screen.height - 50;

        if (fuelPercentage > 0)
            GUI.Label(new Rect(x, y, 400, 100), "Fuel: " + fuelPercentage.ToString("F1") + "%");
        else
            GUI.Label(new Rect(x, y, 400, 100), "Out of fuel!");
    }

    internal override void OnLegacyGUI()
    {
        GUIStyle raceGuiStyle = GUI.skin.label; 

        switch (raceState)
        {
            case RaceState.Starting:
                if (!raceStartTimer.HasStarted())
                    raceStartTimer.Start(RACE_START_TIME);

                Int64 remainingTime = ((raceStartTimer.length - raceStartTimer.GetElapsedTime()) / 1000) + 1; // +1 for "3, 2, 1..."

                raceGuiStyle.fontSize = 72;
                GUI.color = Color.yellow;
                GUI.Label(new((Screen.width / 2) - 20, (Screen.height / 2 - 50), 40, 100), remainingTime.ToString(), raceGuiStyle);

                if (raceStartTimer.IsDone())
                {
                    // since there is no car selection menu
                    CarManager.SetPlayerCar("CarBasic");
                    raceState = RaceState.Active;

                }
                break;
            case RaceState.Active:
                // draw various uis here
                DrawTimer(raceGuiStyle);
                DrawFuelGauge(raceGuiStyle);
                DrawMoneyAmount(raceGuiStyle);
                break;
        }
    }
}