using System;
using System.Collections.Generic;
using UnityEngine;

//
// Entrance point for car system. Called from GameModeInit
//
internal static class CarManager
{
    private const string CAR_FILE_EXTENSION = ".txt";

    /* these basically get spawned into the world based on templates stored here */
    internal static List<Car> cars;

    internal static void Init()
    {
        cars = new();

        string[] fileNames = FileUtils.GetAssetPathsForDirectory("Assets/Resources/Cars", CAR_FILE_EXTENSION);

        foreach (string fileName in fileNames)
        {
            Car car = new();
            car.config = AssetManager.LoadAsset<TextAsset>(fileName);
            car.LoadConfig();
            cars.Add(car);
        }
    }
}