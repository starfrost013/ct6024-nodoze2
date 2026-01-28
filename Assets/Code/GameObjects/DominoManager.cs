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

        string[] fileNames = FileUtils.GetAssetPathsForDirectory("Domino", DOMINO_FILE_EXTENSION);

        foreach (string fileName in fileNames)
        {
            Debug.Log("Loading domino at " + fileName);
            Domino domino = new(fileName); // create and load the domino
            dominoes.Add(domino);
        }
    }
}