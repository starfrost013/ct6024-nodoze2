using System;
using UnityEngine;

// This thingy is a connector between unity and our fancy stuff

public class GameManagerObject : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
        string dateTime = "**** Alpha - Testing ****\n" + Application.version + " (Unity " + Application.unityVersion + ")\n" + DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss");

        // make the font a bit larger
        GUIStyle style = GUI.skin.label;
        style.fontSize = 20;

        GUI.Label(new Rect(5, 5, 400, 100), dateTime, style);
    }
}
