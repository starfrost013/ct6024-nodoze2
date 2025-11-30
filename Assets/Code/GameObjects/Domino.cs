using System;
using UnityEngine;

//
// Domino base definitio
//
internal class Domino : MonoBehaviour
{
    // 
    // STRUCTS
    //

    //
    // FIELDS
    //

    internal string dominoName; // UnityEngine.Object has name
    internal string description;

    private TextAsset config;

    //
    // METHODS
    //

    internal void Start()
    {
        // unity can't load custom resource types...we'll have to replace this
        config = AssetManager.LoadAsset<TextAsset>("Domino/DominoTest");

        // we only ever load values from our cfg's at load time
        ConfigParser.ParseCfg(config.text);

        dominoName = ConfigParser.GetValue("Info", "Name");
        description = ConfigParser.GetValue("Info", "Description");

    }

    internal void Update()
    {
        // Temporary until our super fancy camera systm is created


    }

}
