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

    internal string name;
    internal string description;

    private GameAssetConfigFile config;

    //
    // METHODS
    //

    internal void Start()
    {
        config = AssetManager.LoadAsset<GameAssetConfigFile>("/Domino/DominoTest.cfg");
        config.ParseCfg();

        config.name = config.GetValue("Info", "Name");
        config.name = config.GetValue("Info", "Description");

    }

    internal void Update()
    {

    }

}
