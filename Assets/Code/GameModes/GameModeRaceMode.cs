using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

/// The main race mode game mode.
internal class GameModeRaceMode : GameMode
{
    /// <summary>
    /// prefix for race config path
    /// </summary>
    internal const string RACE_CONFIG_PATH = "Races/";

    private TextAsset raceConfigDataText { get; set; }
    
    //
    // various timers
    //

    /// <summary>
    /// The race event 
    /// </summary>
    internal Race race { get; private set; }

    private bool LoadEventData()
    {
        if (string.IsNullOrWhiteSpace(ProgressionCoordinator.currentLevel.scene)) // ok becuase this only runs once per level load, otherwise use equality
        {
            Debug.LogError("**** Progression data is INVALID **** - NumLevels not high enough...");
            return false; 
        }

        Debug.Log("Loading event data for level " + ProgressionCoordinator.currentLevel.scene);
        string levelConfigDataPath = RACE_CONFIG_PATH + ProgressionCoordinator.currentLevel.scene;

        // load the race information
        raceConfigDataText = AssetManager.LoadAsset<TextAsset>(levelConfigDataPath);
        ConfigParser.Parse(raceConfigDataText.text);

        // TODO: Move this to its own thing

        if (!Enum.TryParse(ConfigParser.GetValue("EventType"), out RaceEventType eventType))
            Debug.LogWarning("No event type specified for level " + ProgressionCoordinator.currentLevel.scene + " ,assuming regular race");

        switch (eventType)
        {
            case RaceEventType.Race:
                race = new RaceBasic();
                break; 
            case RaceEventType.Boss:
                race = new RaceBoss1();
                break; 
        }

        if (race == null)
        {
            Debug.LogError("INVALID Event type specified for level " + ProgressionCoordinator.currentLevel.scene + " : VERY BAD!!!!");
            return false; // don't bother as it will crash
        }

        // just log and continue
        if (!raceConfigDataText)
        {
            Debug.LogError("Failed to load race configuration data for level " + ProgressionCoordinator.currentLevel.scene + " (path " + levelConfigDataPath + ")");
            return false; // don't bother
        }

        bool success = long.TryParse(ConfigParser.GetValue("TimeLimit"), out race.timeLimit)
            | long.TryParse(ConfigParser.GetValue("CheckpointTimeGain"), out race.checkpointTimeGain)
            | long.TryParse(ConfigParser.GetValue("CompletionReward"), out race.completionReward)
            | float.TryParse(ConfigParser.GetValue("KillFloorY"), out race.killFloorY)
            | long.TryParse(ConfigParser.GetValue("RaceStartTime"), out race.raceStartTime)
            | long.TryParse(ConfigParser.GetValue("RestartTimeFail"), out race.restartTimeFail)
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

            race.timeBonuses.Add(new TimeBonusSet
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

        ProgressionCoordinator.AdvanceNormal();

        LoadEventData();
    }

    internal override void OnFrame()
    {

    }

    internal override void OnFixedUpdate()
    {
        race.FixedUpdate();
    }


    internal override void OnLeave()
    {
        
    }

    // TEMPORARY UI LEFT OVER FROM VERSION 1.X - REPLACE IN 2.0

    // todo: This code is *HORRIBLE* 
    internal override void OnLegacyGUI()
    {   
        race.OnGUI(GUI.skin.label);
    }
}