using System;
using System.Collections.Generic;
using UnityEngine;
//
// Entrance point for domino system. Called from GameModeInit
//
internal static class DominoManager
{
    private const string DOMINO_FILE_EXTENSION = "*.txt";

    internal static List<Domino> dominoes;

    internal static void Init()
    {
        dominoes = new();

        TextAsset[] configFiles = AssetManager.LoadAssetsInFolder<TextAsset>("Domino");

        foreach (TextAsset configFile in configFiles)
        {
            Debug.Log("Loading domino at " + configFile.name);
            Domino domino = new();
            domino.config = configFile;
            dominoes.Add(domino);
        }
    }
}