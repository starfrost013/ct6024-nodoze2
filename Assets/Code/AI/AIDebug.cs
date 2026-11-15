using System.Collections.Generic;
using UnityEngine;

//
// The artificial intelligence debug system
// Insert into primary scene...
//


static class AIDebug
{
    private static ImguiInputBox inputBox = new();

    public static void DrawDebugUIWindow(int windowId)
    {
        GUI.Label(new(0, 20, 100, 20), "Select Agent"); 

        inputBox.position = new(0, 40);
        inputBox.size = new(200, 400);
        inputBox.Update();
    }
    // actually dogshit code

    public static void Add(string name) => inputBox.items.Add(name);
    public static void Remove(string name) => inputBox.items.Remove(name);
    public static void OnGUI()
    {
        //maybe we need to update less...
        float width = 450, height = 300;

        // we don't need any extra id
        GUI.Window(1, new Rect(Screen.width - width - 10, Screen.height - height - 10, width, height),
            DrawDebugUIWindow, "AI Debugging System");
    }
}; 