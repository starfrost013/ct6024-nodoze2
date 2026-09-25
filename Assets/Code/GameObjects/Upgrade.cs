using System;
using UnityEngine;

//
// Domino base definitions
//
internal class Upgrade
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

    // maybe we could have multipliers for individual cost

    internal CarModifier modifiers = new();

    internal TextAsset config;

    //
    // METHODS
    //

    public Upgrade(TextAsset config)
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
        // for these ones we don't care

        modifiers.Load();

        // duplicated for use in various other places 

        modifiers.modifierName = name;
        modifiers.internalModifierName = internalName;
    }
}
