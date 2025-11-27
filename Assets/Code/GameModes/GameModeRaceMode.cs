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

    internal override void OnEnter()
    {
        Debug.Log("Entering race...");
    }

    internal override void OnFrame()
    {

    }

    internal override void OnLeave()
    {

    }
}