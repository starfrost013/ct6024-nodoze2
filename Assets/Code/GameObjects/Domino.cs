using System;
using UnityEngine;

//
// Domino base definitions
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

    internal TextAsset config;

    //
    // METHODS
    //

    // Run befor eloading
    internal void LoadConfig()
    {
        // we only ever load values from our cfg's at load time
        ConfigParser.Parse(config.text);

        dominoName = ConfigParser.GetValue("Info", "Name");
        description = ConfigParser.GetValue("Info", "Description");
    }

    void Start()
    {

    }

    void Update()
    {
        // Temporary until our super fancy camera system is created


    }

}
