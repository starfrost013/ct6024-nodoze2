using System;
using UnityEngine;

// This thingy is a connector between unity and our fancy stuff

public class GameManagerObject : MonoBehaviour
{
    /* temp */ 
    Timer gameTimer = new(); 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Start(this);
        gameTimer.Start(Timer.TIMER_CONTINUE_FOREVER);
    }

    // Update is called once per frame
    void Update()
    {
        GameManager.OnFrame();        
    }

    private void FixedUpdate()
    {
        GameManager.OnFixedUpdate();
    }

    private void OnGUI()
    {
        string dateTime = "**** Alpha - Testing ****\n" + Application.version + " (Unity " + Application.unityVersion + ")\n" + DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss");

        // make the font a bit larger
        GUIStyle style = GUI.skin.label;
        style.fontSize = 20;

        GUI.Label(new Rect(5, 5, 400, 100), dateTime, style);

        //maybe we need to update less...

        Int64 totalTime = gameTimer.GetElapsedTime();

        // easier to use constants. 60000 seconds 
        Int64 milliseconds = totalTime % 1000;
        Int64 seconds = (totalTime / 1000) % 60;
        Int64 minutes = ((totalTime / 1000) / 60) % 60;

        string millisecondsString = milliseconds.ToString(), secondsString = seconds.ToString(), minutesString = minutes.ToString();

        // this might be a slow operation
        if (milliseconds < 10)
            millisecondsString = '0' + millisecondsString;

        if (seconds < 10)
            secondsString = '0' + secondsString;

        if (minutes < 10)
            minutesString = '0' + minutesString;

        string timerString = minutesString + ":" + secondsString + ":" + millisecondsString;
        GUI.Label(new Rect(Screen.width - 100, 5, 400, 100), timerString);
    }
}
