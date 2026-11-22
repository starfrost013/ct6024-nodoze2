
using UnityEngine;

/// <summary>
/// Basic race type used for most levels
/// </summary>
class RaceBoss1 : RaceBasic
{
    enum BossPhase
    {
        Spawn,
        Phase1,
        Phase2,
        Phase3,
        Phase4,
        Beaten, 
    };
    
    private BossPhase phase; 

    internal override void Start()
    {
        base.Start();
        phase = BossPhase.Spawn;
    }

    internal override void End()
    {
        
    }

    internal override void OnGUI(GUIStyle style)
    {
        switch (state)
        {
            case RaceState.Starting:
            case RaceState.Countdown:
                base.OnGUI(style);
                break;
        }
    }

    internal override void FixedUpdate()
    {
        
    }
}