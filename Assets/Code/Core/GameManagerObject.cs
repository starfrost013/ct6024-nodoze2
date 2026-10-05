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

    private void OnGUI()
    {
        GameManager.OnLegacyGUI();

        if (GlobalSettings.debugMode)
            GameManagerDebug.DrawDebugUI(buildDate.text);
    }
}
