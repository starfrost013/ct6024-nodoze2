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

    internal string name;               // name
    internal string description;        // description     
    internal string internalName;       // the itnernal name
    internal string requires;           // optional - domino required 
    internal float cost;                // cost of the domino
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
        internalName = config.name;
        cost = float.Parse(ConfigParser.GetValue("Info", "Cost"));
        expiryTime = float.Parse(ConfigParser.GetValue("Info", "ExpiryTime"));
        requires = ConfigParser.GetValue("Info", "Requires");
        modifiers.Load();

        // duplicated for use in various other places 

        modifiers.name = name;
        modifiers.internalName = internalName;
    }
}
