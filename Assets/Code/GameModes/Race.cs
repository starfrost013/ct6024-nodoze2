using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

/// The main race mode game mode.


internal enum RaceState
{
    Starting = 0,               // Race was started
    Countdown = 1,              // Race countdown is active
    Active = 2,                 // Race is active
    FinishedNormal = 3,         // Race was finished successfully
    FinishedSpecial = 4,        // Race was finished using a special exit
    Failed = 5,                 // Race was failed
};


/// <summary>
/// A time bonus set.
/// </summary>
internal struct TimeBonusSet
{
    internal long timerMax;
    internal long moneyGranted;
};

internal enum RaceEventType
{
    /// <summary>
    /// a normal race
    /// </summary>
    Race = 0,

    /// <summary>
    /// a boss race
    /// </summary>
    Boss = 1,
};

internal enum RaceFailReason
{ 
    OutOfFuel = 0,              // Ran out of fuel
    OutOfTime = 1,              // Ran out of time
    OutOfMap = 2,               // Ran out of the map
};

/// <summary>
/// NEW race system
/// 
/// Allows multiple type of races
/// </summary>
abstract class Race
{    
    /// <summary>
    /// prefix for race config path
    /// </summary>
    protected const string RACE_CONFIG_PATH = "Races/";
    internal const int TIME_LIMIT_NONE = -1;            // no time limit
    public RaceState state { get; set; } = RaceState.Starting; // todo: make this set accessor do something in later refacornig so we don't have to access this
    public RaceFailReason failReason { get; protected set; }

    // Due to some limitations with the configuration system, everything has to be here
    internal long timeLimit;                            // time limit, adjusted by checkpoints
    internal long checkpointTimeGain;                   // amount you gain per checkpoint
    internal long completionReward;                     // completion reward
    internal long raceStartTime;                        // countdown time
    internal long restartTimeFail;

    public float killFloorY;                            // kill floor positon
    internal List<TimeBonusSet> timeBonuses = new();    // is this slow?   
    internal RaceEventType eventType; 
    internal Timer raceStartTimer = new();               // the race countdown timer
    internal Timer raceTimer = new();                    // race timer
    internal Timer raceFailTimer = new();                // the race fail timer
    protected bool countdown3Done = false, countdown2Done = false, countdown1Done = false;
    
    protected const string INVENTORY_UI_NAME = "InventoryUI";

    internal virtual void Start()
    {
        CarManager.SpawnPlayerCar();
        state = RaceState.Active;

        // enable race and inventory ui. if it is disabled add it

        if (UIManager.GetUIByName(INVENTORY_UI_NAME) != null)
            UIManager.EnableUI(INVENTORY_UI_NAME);
        else
            UIManager.AddUI(INVENTORY_UI_NAME);
    }

    private void UpdateCountdown()
    {
        // remaining time in seconds
        long remainingTime = ((raceStartTimer.length - raceStartTimer.GetElapsedTime()) / 1000) + 1; // +1 for "3, 2, 1..."

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
            Start();
        }    
    }
    
    internal virtual void FixedUpdate()
    {
        switch (state)
        {
            case RaceState.Countdown:
                UpdateCountdown();
                break;
        }
    }

    internal virtual void End()
    {
        GameManager.player.carInWorld.disableInputs = false;                 //just in case
        UIManager.DisableUI(INVENTORY_UI_NAME);                                 // no need to disable it we can put it back
        GameManager.SetGameState(GameManager.GameModeEnum.RaceFinished);
    }

    internal abstract void OnGUI(GUIStyle style);

    internal virtual void Fail(RaceFailReason failReason, long time = 5000)
    {
        if (state == RaceState.Failed)
            return;

        this.failReason = failReason;
        state = RaceState.Failed;

        // turn off the car's inputs
        GameManager.player.carInWorld.disableInputs = true;
        raceFailTimer.Start(time);
    }

    /// <summary>
    /// shared between all race types, convenience thing
    /// </summary>
    /// <param name="raceGuiStyle">the Unity IMGUI style to draw with [TEMP]</param>
    protected virtual void DrawCountdownUI(GUIStyle raceGuiStyle)
    {
        //todo: don't need to find the car start twice.
        GameObject start = GameObject.Find(CarManager.OBJECT_NAME_CAR_START);

        Camera.main.transform.position = start.transform.position + new Vector3(0.0f, 1.0f, 0.0f);
        Camera.main.transform.localEulerAngles = new Vector3(start.transform.rotation.eulerAngles.x, start.transform.rotation.eulerAngles.y + 180.0f, start.transform.rotation.eulerAngles.z); // stupi car model nonsense !!!

        // remaining time in seconds
        long remainingTime = ((raceStartTimer.length - raceStartTimer.GetElapsedTime()) / 1000) + 1; // +1 for "3, 2, 1..."

        raceGuiStyle.fontSize = 72;
        GUI.color = Color.cornflowerBlue;

        if (remainingTime <= (raceStartTime / 1000))
            GUI.Label(new((Screen.width / 2) - 20, (Screen.height / 2 - 50), 40, 100), remainingTime.ToString(), raceGuiStyle);

    }

}