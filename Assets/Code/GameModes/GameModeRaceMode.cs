using UnityEngine;

// The main race mode game mode/.
internal class GameModeRaceMode : GameMode
{

    enum RaceState
    {
        Starting = 0,
        Active = 1,
        Finished = 2,
    };

    RaceState raceState;


    internal string raceConfigFile;

    internal override void OnEnter()
    {
        Debug.Log("Entering race...");
    }

    internal override void OnFrame()
    {
        switch (raceState)
        {
            case RaceState.Starting:
                
                break;
        }
    }

    internal override void OnFixedUpdate()
    {
        
    }

    internal override void OnLeave()
    {

    }
}