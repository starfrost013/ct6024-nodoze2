using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This thingy is a connector between unity and our fancy stuff
/// Also it has the debug stuff, as the root monobehaviour of the game.
/// </summary>
public class GameManagerObject : MonoBehaviour
{
    private TextAsset buildDate; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {            
        buildDate = AssetManager.LoadAsset<TextAsset>(BuildDate.BUILD_DATE_PATH);
        GameManager.Start(this);
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

    private void DrawDebugUIWindow(int windowId)
    {
        GUIStyle debugGuiStyleLabel = GUI.skin.label;
        GUIStyle debugGuiStyleButton = GUI.skin.button;

        debugGuiStyleLabel.fontSize = debugGuiStyleButton.fontSize = 12;

        GUI.Label(new Rect(10, 30, 250, 25), "Game State = " + GameManager.GetGameState().ToString(), debugGuiStyleLabel);

        if (GUI.Button(new Rect(10, 60, 150, 25), "Finish Current Level", debugGuiStyleButton))
        {
            // do a proper transition
            if (GameManager.GetGameState() == GameManager.GameModeEnum.RaceMode)
            {
                GameModeRaceMode raceMode = (GameModeRaceMode)GameManager.mode;
                raceMode.raceState = GameModeRaceMode.RaceState.Finished;
            }
            else
            {
                GameManager.SetGameState(GameManager.GameModeEnum.RaceFinished);
            }
        }

        if (GUI.Button(new Rect(260, 30, 70, 25), "Close", debugGuiStyleButton))
            GlobalSettings.debugMode = false;
    }

    private void DrawDebugUI()
    {
        if (!GlobalSettings.debugMode)
            return;

        // first draw the version information
        string dateTime = Application.version + " (Unity " + Application.unityVersion + ")\n" + 
            "Build Date: " + buildDate.text + "\nTest Date: " + DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss");

        // make the font a bit larger
        GUIStyle style = GUI.skin.label;
        style.fontSize = 20;
        GUI.color = Color.white; 

        GUI.Label(new Rect(5, 5, 400, 100), dateTime, style);

        //maybe we need to update less...
        float width = 350, height = 200;

        // we don't need any extra id
        GUI.Window(0, new Rect(10, Screen.height - height - 10, width, height),
            DrawDebugUIWindow, "It's a Bug! (Debug Display)");
;    }

    private void OnGUI()
    {
        GameManager.OnLegacyGUI();

        if (GlobalSettings.debugMode)
            DrawDebugUI();
    }
}
