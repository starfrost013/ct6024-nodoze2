using System;
using System.Collections.Generic;
using UnityEngine;
//
// Entrance point for domino system. Called from GameModeInit
//
internal static class DominoManager
{
    private const string DOMINO_FILE_EXTENSION = ".txt";

    internal static List<Domino> dominoes;

    internal static void Init()
    {
        string[] fileNames = FileUtils.GetAssetPathsForDirectory("Assets/Resources/Domino", DOMINO_FILE_EXTENSION);

        foreach (string fileName in fileNames)
        {
            Domino domino = new();
            domino.config = AssetManager.LoadAsset<TextAsset>(fileName);
            domino.LoadConfig();
            dominoes.Add(domino);
        }
    }
}