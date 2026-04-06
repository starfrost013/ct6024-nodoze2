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

    internal string name;
    internal string description;
    internal string required;           // optional - domino required 
    internal float cost;
    internal float expiryTime;          // 0 =none, for temporary powerups
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
        required = ConfigParser.GetValue("Info", "Required");
        modifiers.Load();

        // duplicated for use in various other places 

        modifiers.name = name;
    }
}
