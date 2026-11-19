
using UnityEngine;

/// <summary>
/// Basic race type used for most levels
/// </summary>
class RaceBasic : Race
{
    
    internal override void End()
    {
        OnFinishCalculateTimeBonus();                                        // calculate time bonus based on race configuration
        base.End();
    }

    /// <summary>
    /// draws the race fail reason if it exists
    /// </summary>
    private void DrawFailReason(GUIStyle raceGuiStyle)
    {
        string failString = "Generic Failure String";

        switch (failReason)
        {
            case RaceFailReason.OutOfFuel:
                failString = "Out of fuel!";
                break;
            case RaceFailReason.OutOfTime:
                failString = "Out of time!";
                break;
            case RaceFailReason.OutOfMap:
                failString = "Down you go...!";
                break;
        }
        
        int width = 330, height = 100;
        raceGuiStyle.fontSize = 48;
        GUI.color = Color.red;

        GUI.Label(new Rect(Screen.width / 2 - (width / 2), (Screen.height / 2) - (height / 2), width, height), failString, raceGuiStyle);
    }

    private void DrawTimer(GUIStyle raceGuiStyle)
    {
        if (!raceTimer.HasStarted())
            raceTimer.Start(Timer.TIMER_CONTINUE_FOREVER);

        long totalTime = 0;

        if (timeLimit == TIME_LIMIT_NONE)
            totalTime = raceTimer.GetElapsedTime();
        else
            totalTime = timeLimit - raceTimer.GetElapsedTime();

        string timerString = string.Empty;

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

        GUI.color = Color.darkGray;

        // draw a box
        GUI.Box(new Rect(0, 0, Screen.width, 90), "");

        Color newColor = new(1.0f, 0.9f, 0.9f, 1.0f);
        GUI.color = newColor;
        raceGuiStyle.fontSize = 48;

        int width = 250, height = 100;

        GUI.Label(new Rect(Screen.width / 2 - (width / 2), 0, width, height), timerString, raceGuiStyle);
    }

    private void DrawMoneyAmount(GUIStyle raceGuiStyle)
    {
        Car car = GameManager.player.carInWorld;
        GUI.color = Color.lawnGreen;
        raceGuiStyle.alignment = TextAnchor.UpperRight;

        float x = Screen.width - 410;
        float y = 40;

        raceGuiStyle.fontSize = 36;

        GUI.Label(new Rect(x, y, 400, 100), "Money: $" + GameManager.player.stats.money, raceGuiStyle);
        //restore alignment (hack - but this code is going away soon anyway)
        raceGuiStyle.alignment = TextAnchor.UpperLeft;

    }

    private void DrawFuelGauge(GUIStyle raceGuiStyle)
    {
        Car car = GameManager.player.carInWorld;
        GUI.color = Color.cornflowerBlue;
        raceGuiStyle.alignment = TextAnchor.UpperRight;
        raceGuiStyle.fontSize = 36;

        float fuelPercentage = (car.physics.fuelCurrent / car.GetCarModifiers().fuelMax) * 100;

        float x = Screen.width - 640;
        float y = 0;

        bool dispOutOfFuel = (state == RaceState.Failed && failReason == RaceFailReason.OutOfFuel);

        // ALSO displayed by Fail screen but whaatever
        if (!dispOutOfFuel)
        {
            GUI.Label(new Rect(x, y, 400, 100), "Fuel: ", raceGuiStyle );

            int numToDisplay = (int)(fuelPercentage / 10) + 1;
            
            for (int i = 0; i < numToDisplay; i++)
            {
                GUI.Label(new Rect(Screen.width - 24 - (i * 24), y, 20, 60), "█", raceGuiStyle);
            }
        }
        else
            GUI.Label(new Rect(x, y, 400, 100), "Out of fuel!");

            //restore alignment (hack - but this code is going away soon anyway)
        raceGuiStyle.alignment = TextAnchor.UpperLeft;
    }



    internal override void OnGUI(GUIStyle style)
    {
        GUIStyle raceGuiStyle = GUI.skin.label; 

        switch (state)
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

                if (state == RaceState.Failed)
                    DrawFailReason(raceGuiStyle);
                else
                {
                    // boss events don't have timer
                    if (eventType == RaceEventType.Race)
                        DrawTimer(raceGuiStyle);
                }
                DrawFuelGauge(raceGuiStyle);
                DrawMoneyAmount(raceGuiStyle);
                break;

            // the race is done
        }   
    }
    private void OnFinishCalculateTimeBonus()
    {
        float elapsedTime = raceTimer.GetElapsedTime();

        // appyl the specified time bonus
        for (int i = 0; i < timeBonuses.Count - 1; i++)     
        {
            TimeBonusSet thisTimeBonus = timeBonuses[i]; 
            TimeBonusSet nextTimeBonus = timeBonuses[i + 1];

            bool awardBonus = (elapsedTime > thisTimeBonus.timerMax
            && elapsedTime < nextTimeBonus.timerMax);

            if (i == 0)
                awardBonus = elapsedTime < thisTimeBonus.timerMax;

            if (awardBonus)
                GameManager.player.stats.money += thisTimeBonus.moneyGranted;
        }

        //grant them the completion reward
        GameManager.player.stats.money += completionReward;
    }

    internal override void FixedUpdate()
    {
        switch (state)
        {
            case RaceState.Starting:
                if (eventType == RaceEventType.Race)
                {
                    if (raceTimer.HasStarted())
                        raceTimer.Restart();
                }   

                if (raceStartTimer.HasStarted())
                    raceStartTimer.Restart();
                else
                    raceStartTimer.Start(raceStartTime);

                state = RaceState.Countdown;

                break;
            // the countdown state
            case RaceState.Countdown:
                break;
            // the race is active
            case RaceState.Active:
                // check if we are out of time

                if (timeLimit != TIME_LIMIT_NONE
                && eventType == RaceEventType.Race)
                {
                    long totalTime = timeLimit - raceTimer.GetElapsedTime();

                    if (totalTime < 0)
                        Fail(RaceFailReason.OutOfTime); 
                }

                break;
            case RaceState.Failed:
                //cheap way of resetting the current level 
                if (raceFailTimer.GetElapsedTime() >= restartTimeFail)
                {
                    raceFailTimer.Reset();
                    state = RaceState.Starting;
                }

                break;
            // the race is done
            case RaceState.FinishedNormal:
            case RaceState.FinishedSpecial:
                End();
                break;
        }       
    }
}