using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// The main race mode game mode/.
internal class GameModeRaceMode : GameMode
{

    /// <summary>
    /// restart timer time when you fail the race
    /// </summary>
    internal const long RESTART_TIME_FAIL = 5000;

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
        FinishedNormal = 3,         // Race was finished successfully
        FinishedSpecial = 4,        // Race was finished using a special exit
        Failed = 5,                 // Race was failed
    };

    internal enum RaceFailReason
    { 
        OutOfFuel = 0,              // Ran out of fuel
        OutOfTime = 1,              // Ran out of time
        OutOfMap = 2,               // Ran out of the map
    };

    private RaceFailReason raceFailReason;  // the reason that we failed. only valid if racestate == racestate::failed

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
        internal const int TIME_LIMIT_NONE = -1;            // no time limit

        internal long timeLimit;                            // time limit, adjusted by checkpoints
        internal long checkpointTimeGain;
        internal long completionReward;
        internal float killFloorY;                          // kill floor positon

        internal List<TimeBonusSet> timeBonuses = new(); // is this slow?   
    };

    
    /// <summary>
    /// externally accessed property -- the substate of the race
    /// </summary>
    internal RaceState raceState { get; set; }

    private TextAsset raceConfigDataText { get; set; }
    internal RaceConfigData raceConfigData { get; private set; }
    
    //
    // various timers
    //

    /// <summary>
    /// Timer used during the countdown
    /// </summary>
    Timer raceStartTimer = new();

    // yeah i modify this directly but i have to demo this in less than 24 hours
    /// <summary>
    /// Timer used to actually play the game.
    /// </summary>
    internal Timer raceTimer = new(); 

    /// <summary>
    /// Timer used to restart things
    /// </summary>
    Timer raceFailTimer = new(); 


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
            | float.TryParse(ConfigParser.GetValue("KillFloorY"), out raceConfigData.killFloorY)
            ;

        // still at least try to load
        if (!success)
            Debug.LogError("Event data for " + ProgressionCoordinator.currentLevel.scene + " must at least have a time limit, completion reward, kill floor Y and checkpoint time gain!");

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

    }

    internal override void OnFixedUpdate()
    {


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
                // check if we are out of time

                if (raceConfigData.timeLimit != RaceConfigData.TIME_LIMIT_NONE)
                {
                    long totalTime = raceConfigData.timeLimit - raceTimer.GetElapsedTime();

                    if (totalTime < 0)
                        FailRace(RaceFailReason.OutOfTime); 
                }

                break;
            case RaceState.Failed:
                //cheap way of resetting the current level 
                if (raceFailTimer.GetElapsedTime() >= RESTART_TIME_FAIL)
                {
                    raceFailTimer.Reset();
                    raceState = RaceState.Starting;
                }

                break;
            // the race is done
            case RaceState.FinishedNormal:
                OnFinishCalculateTimeBonus();                                        // calculate time bonus based on race configuration
                GameManager.player.carInWorld.disableInputs = false;                 //just in case
                GameManager.SetGameState(GameManager.GameModeEnum.RaceFinished);
                break;
        }

    }

    private void OnFinishCalculateTimeBonus()
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

        long totalTime = 0;

        if (raceConfigData.timeLimit == RaceConfigData.TIME_LIMIT_NONE)
            totalTime = raceTimer.GetElapsedTime();
        else
            totalTime = raceConfigData.timeLimit - raceTimer.GetElapsedTime();

        string timerString = string.Empty;

        bool displayOutOfTime = (raceState == RaceState.Failed && raceFailReason == RaceFailReason.OutOfTime);
        
        if (!displayOutOfTime)
        {
            // easier to use constants. 60000 seconds 
            long milliseconds = totalTime % 1000;
            long seconds = (totalTime / 1000) % 60;
            long minutes = ((totalTime / 1000) / 60) % 60;

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


            timerString = minutesString + ":" + secondsString + "." + millisecondsString;
        }
        else
            timerString = "Out of time!";

        Color newColor = new(1.0f, 0.1f, 0.1f, 255);
        GUI.color = newColor;
        raceGuiStyle.fontSize = 48;

        int width = 250, height = 100;

        GUI.Label(new Rect(Screen.width / 2 - (width / 2), 0, width, height), timerString, raceGuiStyle);
    }

    private void DrawMoneyAmount(GUIStyle raceGuiStyle)
    {
        Car car = GameManager.player.carInWorld;
        GUI.color = Color.green;
        raceGuiStyle.alignment = TextAnchor.UpperRight;

        float x = Screen.width - 410;
        float y = Screen.height - 90;

        raceGuiStyle.fontSize = 36;

        GUI.Label(new Rect(x, y, 400, 100), "Money: $" + GameManager.player.stats.money, raceGuiStyle);
        //restore alignment (hack - but this code is going away soon anyway)
        raceGuiStyle.alignment = TextAnchor.UpperLeft;

    }

    private void DrawFuelGauge(GUIStyle raceGuiStyle)
    {
        Car car = GameManager.player.carInWorld;  
        GUI.color = Color.blue;
        raceGuiStyle.alignment = TextAnchor.UpperRight;
        raceGuiStyle.fontSize = 36;

        float fuelPercentage = (car.physics.fuelCurrent / car.GetCarModifiers().fuelMax) * 100;

        float x = Screen.width - 410;
        float y = Screen.height - 50;

        bool dispOutOfFuel = (raceState == RaceState.Failed && raceFailReason == RaceFailReason.OutOfFuel);

        if (!dispOutOfFuel)
            GUI.Label(new Rect(x, y, 400, 100), "Fuel: " + fuelPercentage.ToString("F1") + "%");
        else
            GUI.Label(new Rect(x, y, 400, 100), "Out of fuel!");

            //restore alignment (hack - but this code is going away soon anyway)
        raceGuiStyle.alignment = TextAnchor.UpperLeft;
    }

    private void DrawCountdownUI(GUIStyle raceGuiStyle)
    {
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
            AudioManager.PlayAudioAtCameraPosition("Announcer_Countdown3", 1.0f);
        }
        else if (remainingTime < 3
            && !countdown2Done)
        {
            countdown2Done = true;
            AudioManager.PlayAudioAtCameraPosition("Announcer_Countdown2", 1.0f);
        }
        else if (remainingTime < 2
            && !countdown1Done)
        {
            countdown1Done = true;
            AudioManager.PlayAudioAtCameraPosition("Announcer_Countdown1", 1.0f);
        }

        if (raceStartTimer.IsDone())
        {
            AudioManager.PlayAudioAtCameraPosition("Announcer_CountdownGO", 1.0f);
            // since there is no car selection menu
            CarManager.SpawnPlayerCar();
            raceState = RaceState.Active;
        }
    }

    internal void FailRace(RaceFailReason failReason, long time = RESTART_TIME_FAIL)
    {
        if (raceState == RaceState.Failed)
            return;

        raceFailReason = failReason;
        raceState = RaceState.Failed;

        // turn off the car's inputs
        GameManager.player.carInWorld.disableInputs = true;
        raceFailTimer.Start(time);
    }

    internal override void OnLeave()
    {
    }

    // todo: This code is *HORRIBLE* 
    internal override void OnLegacyGUI()
    {
        GUIStyle raceGuiStyle = GUI.skin.label; 

        switch (raceState)
        {
            // the countdown state
            case RaceState.Countdown:
                DrawCountdownUI(raceGuiStyle);
                break;
            // the race is active
            case RaceState.Active:
            case RaceState.Failed:
            case RaceState.FinishedNormal:
            case RaceState.FinishedSpecial:
                // draw various uis here
                DrawTimer(raceGuiStyle);
                DrawFuelGauge(raceGuiStyle);
                DrawMoneyAmount(raceGuiStyle);
                break;
            // the race is done
        }
    }
}