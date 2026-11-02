using System.Collections.Generic;
using UnityEngine;

//
// The artificial intelligence debug system
// Insert into primary scene...
//


static class AIDebug
{
    public static void DrawDebugUIWindow(int windowId)
    {
        
    }

    public static void OnGUI()
    {
        //maybe we need to update less...
        float width = 450, height = 300;

        // we don't need any extra id
        GUI.Window((int)GameManager.DebugFlags.DebugAI, new Rect(10, Screen.height - height - 10, width, height),
            DrawDebugUIWindow, "AI Debugging System");
    }
}; 