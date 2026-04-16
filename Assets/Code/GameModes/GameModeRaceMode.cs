using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

// The main race mode game mode/.
internal class GameModeRaceMode : GameMode
{

    /// <summary>
    /// restart timer time when you run out o fuel
    /// </summary>
    internal const long RESTART_TIME_OUT_OF_FUEL = 5000;

    /// <summary>
    /// prefix for race config path
    /// </summary>
    internal const string RACE_CONFIG_PATH = "Races/";

    /* At some point we need to put all this in a config file */
    const int RACE_START_TIME = 3000;

    internal enum RaceState
    {
        Starting = 0,               // Race was started
        Countdown = 1,              // Race countdown is active
        Active = 2,                 // Race is active
        Finished = 3,               // Race was finished successfully
        Failed = 4,                 // Race was failed
    };

    /// <summary>
    /// A time bonus set.
    /// </summary>
    internal struct TimeBonusSet
    {
        internal long timerMax;
        internal long moneyGranted;
    };

    /// <summary>
    /// The configuration data of the race.
    /// </summary>
    internal class RaceConfigData
    {
        internal long timeLimit;
        internal long checkpointTimeGain;
        internal long completionReward;

        internal List<TimeBonusSet> timeBonuses = new(); // is this slow?   
    };

    
    // externally accessed property -- the subset of the race
    internal RaceState raceState { get; set; }
    private TextAsset raceConfigDataText { get; set; }
    internal RaceConfigData raceConfigData { get; private set; }
    
    Timer raceStartTimer = new();
    Timer raceTimer = new();

    /// <summary>
    /// Timer used to restart things
    /// </summary>
    Timer restartTimer = new(); 


    // THIS IS A TERRIBLE WAY OF DOING THIS!
    private bool countdown3Done = false, countdown2Done = false, countdown1Done = false;

    private bool LoadEventData()
    {
        Debug.Log("Loading event data for level " + ProgressionCoordinator.currentLevel.scene);
        string levelConfigDataPath = RACE_CONFIG_PATH + ProgressionCoordinator.currentLevel.scene;

        // ensure that osmething happens
        raceConfigData = new();

        // load the race information
        raceConfigDataText = AssetManager.LoadAsset<TextAsset>(levelConfigDataPath);
        ConfigParser.Parse(raceConfigDataText.text);

        // just log and continue
        if (!raceConfigDataText)
        {
            Debug.LogError("Failed to load race configuration data for level " + ProgressionCoordinator.currentLevel.scene + " (path " + levelConfigDataPath + ")");
            return false; // don't bother
        }

        bool success = long.TryParse(ConfigParser.GetValue("TimeLimit"), out raceConfigData.timeLimit)
            | long.TryParse(ConfigParser.GetValue("CheckpointTimeGain"), out raceConfigData.checkpointTimeGain)
            | long.TryParse(ConfigParser.GetValue("CompletionReward"), out raceConfigData.completionReward)
            ;

        // still at least try to load
        if (!success)
            Debug.LogError("Event data for " + ProgressionCoordinator.currentLevel.scene + " must at least have a time limit, completion reward! and checkpoint time gain!");

        int.TryParse(ConfigParser.GetValue("NumTimeBonus"), out int numTimeBonuses);

        if (numTimeBonuses == 0)
        {
            Debug.LogError("Please specify the number of time bonuses for " + ProgressionCoordinator.currentLevel.scene + "!");
            return false; 
        }

        for (int i = 0; i < numTimeBonuses; i++)
        {
            success = long.TryParse(ConfigParser.GetValue("TimeBonus" + i), out long currentTimerMax)
            | long.TryParse(ConfigParser.GetValue("TimeBonus" + i + "Money"), out long currentTimerMoney);

            if (!success)
                Debug.LogWarning("Data for time bonus " + i + " of level " + ProgressionCoordinator.currentLevel.scene + "is malformed. Please fix...");

            raceConfigData.timeBonuses.Add(new TimeBonusSet
            { 
                timerMax = currentTimerMax,
                moneyGranted = currentTimerMoney,   
            });
        }

        success = true; 
        return success; 
    }

    internal override void OnEnter()
    {
        // ensure we are in the scene known as race mode
        GameManager.SetCurrentScene(GameManager.SCENE_RACE_MODE);   

        // don't advance level if we failed
        if (raceState != RaceState.Failed)
            ProgressionCoordinator.AdvanceNormal();

        Debug.Log("Entering race...");
        raceState = RaceState.Starting;

        LoadEventData();    

        // wrost hack ever
        if (raceStartTimer.HasStarted())
            raceStartTimer.Start(Timer.TIMER_CONTINUE_FOREVER);
    }

    internal override void OnFrame()
    {
        // TODO: This code is completely broken. The scene doesn't swtich in time for the new CarStart for instance.
        if (Input.GetKeyDown(KeyCode.F7))
            ProgressionCoordinator.AdvanceNormal();

        if (Input.GetKeyDown(KeyCode.F8))
            ProgressionCoordinator.AdvanceSpecial();

    }

    internal override void OnFixedUpdate()
    {
        if (restartTimer.GetElapsedTime() >= RESTART_TIME_OUT_OF_FUEL)
        {
            restartTimer.Reset();
            raceState = RaceState.Failed;
        }

        switch (raceState)
        {
            case RaceState.Starting:
                if (raceTimer.HasStarted())
                    raceTimer.Restart();

                if (raceStartTimer.HasStarted())
                    raceStartTimer.Restart();
                else
                    raceStartTimer.Start(RACE_START_TIME);

                raceState = RaceState.Countdown;
                break;
            // the countdown state
            case RaceState.Countdown:
                break;
            // the race is active
            case RaceState.Active:
                // all this does is draw the ui so this will be moved here
                break;
            case RaceState.Failed:
                //cheap way of resetting the current level 
                raceState = RaceState.Starting;

                break;
            // the race is done
            case RaceState.Finished:
                CalculateTimeBonus();                   // calculate time bonus based on race configuration
                // TEMP. There needs to be a *RACE CONFIGURATION* which will specify the scene to load, etc.
                GameManager.SetGameState(GameManager.GameModeEnum.RaceFinished);
                break;
        }

    }

    internal override void OnLeave()
    {
        ProgressionCoordinator.OnExitRaceScene(); 
    }

    private void CalculateTimeBonus()
    {
        float elapsedTime = raceTimer.GetElapsedTime();

        // appyl the specified time bonus
        for (int i = 0; i < raceConfigData.timeBonuses.Count - 1; i++)     
        {
            TimeBonusSet thisTimeBonus = raceConfigData.timeBonuses[i]; 
            TimeBonusSet nextTimeBonus = raceConfigData.timeBonuses[i + 1];

            bool awardBonus = (elapsedTime > thisTimeBonus.timerMax
            && elapsedTime < nextTimeBonus.timerMax);

            if (i == 0)
                awardBonus = (elapsedTime < thisTimeBonus.timerMax);

            if (awardBonus)
            {
                GameManager.player.stats.money += thisTimeBonus.moneyGranted;
            }
        }

        //grant them the completion reward
        GameManager.player.stats.money += raceConfigData.completionReward;
    }

    private void DrawTimer(GUIStyle raceGuiStyle)
    {
        if (!raceTimer.HasStarted())
            raceTimer.Start(Timer.TIMER_CONTINUE_FOREVER);

        Int64 totalTime = raceTimer.GetElapsedTime();

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
        Car car = GameManager.player.carInWorld;
        GUI.color = Color.green;
        raceGuiStyle.alignment = TextAnchor.UpperRight;

        float fuelPercentage = (GameManager.player.stats.money) * 100;

        float x = Screen.width - 410;
        float y = Screen.height - 90;

        GUI.Label(new Rect(x, y, 400, 100), "Money: $" + GameManager.player.stats.money, raceGuiStyle);

        //restore alignment (hack - but this code is going away soon anyway)
        raceGuiStyle.alignment = TextAnchor.UpperLeft;

    }

    private void DrawFuelGauge(GUIStyle raceGuiStyle)
    {
        Car car = GameManager.player.carInWorld;  
        GUI.color = Color.blue;
        raceGuiStyle.alignment = TextAnchor.UpperRight;

        float fuelPercentage = (car.physics.fuelCurrent / car.GetCarModifiers().fuelMax) * 100;

        float x = Screen.width - 410;
        float y = Screen.height - 50;

        if (fuelPercentage > 0)
            GUI.Label(new Rect(x, y, 400, 100), "Fuel: " + fuelPercentage.ToString("F1") + "%");
        else
        {
            GUI.Label(new Rect(x, y, 400, 100), "Out of fuel!");

            if (!restartTimer.HasStarted())
                restartTimer.Start(RESTART_TIME_OUT_OF_FUEL);
        }

        //restore alignment (hack - but this code is going away soon anyway)
        raceGuiStyle.alignment = TextAnchor.UpperLeft;
    }

    // todo: This code is *HORRIBLE* 
    internal override void OnLegacyGUI()
    {
        GUIStyle raceGuiStyle = GUI.skin.label; 

        switch (raceState)
        {
            // the countdown state
            case RaceState.Countdown:
                // remaining time in seconds
                Int64 remainingTime = ((raceStartTimer.length - raceStartTimer.GetElapsedTime()) / 1000) + 1; // +1 for "3, 2, 1..."

                raceGuiStyle.fontSize = 72;
                GUI.color = Color.yellow;

                if (remainingTime <= (RACE_START_TIME / 1000))
                    GUI.Label(new((Screen.width / 2) - 20, (Screen.height / 2 - 50), 40, 100), remainingTime.ToString(), raceGuiStyle);

                // just hardcode this for now
                if (remainingTime < 4
                    && !countdown3Done)
                {
                    countdown3Done = true;
                    AudioManager.PlayAudioAtPoint("Announcer_Countdown3", Camera.main.transform.position, 1.0f);
                }
                else if (remainingTime < 3
                    && !countdown2Done)
                {
                    countdown2Done = true;
                    AudioManager.PlayAudioAtPoint("Announcer_Countdown2", Camera.main.transform.position, 1.0f);
                }
                else if (remainingTime < 2
                    && !countdown1Done)
                {
                    countdown1Done = true;
                    AudioManager.PlayAudioAtPoint("Announcer_Countdown1", Camera.main.transform.position, 1.0f);
                }

                if (raceStartTimer.IsDone())
                {
                    AudioManager.PlayAudioAtPoint("Announcer_CountdownGO", Camera.main.transform.position, 1.0f);
                    // since there is no car selection menu
                    CarManager.SetPlayerCar("CarBasic");
                    raceState = RaceState.Active;
                }
                break;
            // the race is active
            case RaceState.Active:
                // draw various uis here
                DrawTimer(raceGuiStyle);
                DrawFuelGauge(raceGuiStyle);
                DrawMoneyAmount(raceGuiStyle);
                break;
            // the race is done
            case RaceState.Finished:
                break; 
        }
    }
}