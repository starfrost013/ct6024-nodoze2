using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This is the debug stuff (REFACTORED AGAIN!)
/// </summary>
public static class GameManagerDebug
{
    private static ImguiInputBox inputBox = new();

    internal static void Init()
    {
        inputBox.onItemClicked += LevelSelectButtonPressed;
        inputBox.position = new(10, 120);
        inputBox.size = new(240, 240);
        inputBox.labelName = "Level Select";
    }

    internal static void DrawDebugUIWindow(int windowId)
    {
        // HACK
        if (inputBox.items.Count == 0)
        {
            foreach (var reference in ProgressionCoordinator.levels)
            {
                if (reference.scene != ProgressionCoordinator.NO_MORE_LEVELS)
                    inputBox.items.Add(reference.scene);
            }
        }

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
                raceMode.raceState = GameModeRaceMode.RaceState.FinishedNormal;
            }
            else
                GameManager.SetGameState(GameManager.GameModeEnum.RaceFinished);
        }
        
        if (GUI.Button(new Rect(370, 30, 70, 25), "Close", debugGuiStyleButton))
            GlobalSettings.debugMode = false;

        GUI.Label(new Rect(10, 90, 150, 25), "Level Select", debugGuiStyleLabel);

        inputBox.Update();
    }

    internal static void DrawDebugUI(string buildDate)
    {
        if (!GlobalSettings.debugMode)
            return;

        // first draw the version information
        string dateTime = Application.version + " (Unity " + Application.unityVersion + ")\n" + 
            "Build Date:\t" + buildDate + "\nTest Date:\t" + DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss");

        // make the font a bit larger
        GUIStyle style = GUI.skin.label;
        style.fontSize = 20;
        GUI.color = Color.white; 

        GUI.Label(new Rect(5, 5, 600, 100), dateTime, style);

        //maybe we need to update less...
        float width = 450, height = 300;

        // we don't need any extra id
        GUI.Window(0, new Rect(10, Screen.height - height - 10, width, height),
            DrawDebugUIWindow, "\"It's a Bug!\" Debug System");
    }

    internal static void LevelSelectButtonPressed(object sender, ImguiInputBoxEventArgs e)
    {
        if (e.selectedItem == null)
            return;

        if (GameManager.GetGameState() == GameManager.GameModeEnum.RaceMode)
            ProgressionCoordinator.SetLevel(e.selectedItem);
    }
}