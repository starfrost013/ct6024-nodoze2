using System;
using UnityEngine;

//
// Domino base definitions
//
internal class Domino
{
    // 
    // STRUCTS
    //

    //
    // FIELDS
    //

    internal string dominoName; // UnityEngine.Object has name
    internal string description;

    internal TextAsset config;
    internal string configFilePath;

    //
    // METHODS
    //

    public Domino(string filePath)
    {
        configFilePath = filePath;  
        config = AssetManager.LoadAsset<TextAsset>(configFilePath);

        // we only ever load values from our cfg's at load time
        ConfigParser.Parse(config.text);

        dominoName = ConfigParser.GetValue("Info", "Name");
        description = ConfigParser.GetValue("Info", "Description");
    }
}
