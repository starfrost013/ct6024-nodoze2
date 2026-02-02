using System;
using System.Collections.Generic;
using UnityEngine;
//
// Entrance point for domino system. Called from GameModeInit
//
internal static class DominoManager
{
    internal static List<Domino> dominoes;

    internal static void Init()
    {
        dominoes = new();

        TextAsset[] configFiles = AssetManager.LoadAssetsInFolder<TextAsset>("Domino");

        foreach (TextAsset configFile in configFiles)
        {
            Debug.Log("Loading domino at " + configFile.name);
            Domino domino = new(configFile);
            dominoes.Add(domino);
        }
    }
}