using System;
using System.Collections.Generic;
using UnityEngine;
//
// Entrance point for domino system. Called from GameModeInit
//
internal static class DominoManager
{
    internal static List<Domino> dominoes = new();

    internal static void Init()
    {
        if (dominoes.Count > 0)
            return;

        dominoes = new();

        TextAsset[] configFiles = AssetManager.LoadAssetsInFolder<TextAsset>("Domino");

        foreach (TextAsset configFile in configFiles)
        {
            Debug.Log("Loading domino at " + configFile.name);
            Domino domino = new(configFile);
            dominoes.Add(domino);
        }
    }

    internal static Domino GetDominoByName(string dominoName)
    {
        if (dominoes.Count == 0)
            return null;

        foreach (Domino domino in dominoes)
        {
            if (domino.dominoName == dominoName) 
                return domino;
        }

        return null; // no matching domino
    }
}