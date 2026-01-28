using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using Unity.VisualScripting;
using UnityEngine;

//
// Entrance point for car system. Called from GameModeInit
//
internal static class CarManager
{
    private const string CAR_FILE_EXTENSION = "*.txt";

    /* these basically get spawned into the world based on templates stored here */
    internal static List<Car> cars;

    internal static void Init()
    {
        cars = new();

        string[] fileNames = FileUtils.GetAssetPathsForDirectory("Cars", CAR_FILE_EXTENSION);

        foreach (string fileName in fileNames)
        {
            Debug.Log("Loading car at " + fileName);
            GameObject carObject = AssetManager.LoadAsset<GameObject>(fileName);

            Car carPrefab = carObject.GetComponent<Car>();
    
            if (!carPrefab)
            {
                Debug.LogWarning("No car component in car prefab " + fileName + " " + ", adding one...");
                carPrefab = carObject.AddComponent<Car>();    
            }

            Car car = MonoBehaviour.Instantiate(carPrefab);

            car.transform.position = new(car.transform.position.x, car.transform.position.y + 1.0f, car.transform.position.z);
            car.configFilePath = fileName;
            car.LoadConfig();
            cars.Add(car);

            

        }

        // in the future we'll have a car selection but just load the first car for now
    }
}