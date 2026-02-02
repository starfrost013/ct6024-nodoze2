using UnityEngine;
using UnityEngine.Rendering;

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

    internal string raceConfigFile;

    internal override void OnEnter()
    {
        Debug.Log("Entering race...");
        raceState = RaceState.Starting;
        raceStartTimer.Start(3000);
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

    internal override void OnLegacyGUI()
    {
        GUIStyle raceGuiStyle = GUI.skin.label;

        switch (raceState)
        {
            case RaceState.Starting:
                raceGuiStyle.fontSize = 72;
                GUI.color = Color.red;
                GUI.Label(new((Screen.width / 2) - 50, (Screen.height / 2 - 100), 40, 40), (raceStartTimer.GetElapsedTime() / 1000).ToString(), raceGuiStyle);

                if (raceStartTimer.IsDone())
                    raceState = RaceState.Active;
                break;
        }
    }
}