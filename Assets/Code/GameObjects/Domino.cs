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

    internal string name; // UnityEngine.Object has name
    internal string description;
    internal float cost;
    internal float expiryTime;  // 0 =none, for temporary powerups
    internal CarModifier modifiers = new();

    internal TextAsset config;

    //
    // METHODS
    //

    public Domino(TextAsset config)
    {
        this.config = config;

        // we only ever load values from our cfg's at load time
        ConfigParser.Parse(config.text);

        name = ConfigParser.GetValue("Info", "Name");
        description = ConfigParser.GetValue("Info", "Description");
        cost = float.Parse(ConfigParser.GetValue("Info", "Cost"));
        expiryTime = float.Parse(ConfigParser.GetValue("Info", "ExpiryTime"));
        modifiers.Load();

        // duplicated for car use

        modifiers.name = name;
    }
}
