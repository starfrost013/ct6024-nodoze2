
using System;
using UnityEngine;

/// <summary>
/// Basic race type used for most levels
/// </summary>
class RaceBoss1 : RaceBasic
{
    internal override void Start()
    {
        base.Start();
    }

    internal override void End()
    {
        
    }

    /// <summary>
    /// TEMPORARY BOSSUI drawing code (WILL BE MOVED TO REAL UI LATER MAYBE HOPEFULLY PROBABLY NOT ACTUALLY)
    /// </summary>
    /// <param name="style"></param>
    private void DrawBossUI(GUIStyle style)
    {
        style.fontSize = 22;
        GUI.Label(new Rect(0, 100, 250, 50), "Remaining Enemies: " + AIManager.GetNumAgentsOfType<EnemyCarAgent>());
    }

    internal override void OnGUI(GUIStyle style)
    {
        switch (state)
        {
            case RaceState.Starting:
            case RaceState.Countdown:
                base.OnGUI(style);
                break;
            case RaceState.Active:
                DrawMoneyAmount(style);
                DrawFuelGauge(style);
                DrawBossUI(style);
                break;
        }
    }

    internal override void FixedUpdate()
    {
        switch (state)
        {
            case RaceState.Starting:
            case RaceState.Countdown:
                base.FixedUpdate();
                break;
        }
    }
}