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
    // change this when we have assetbundles
    private const string CAR_PATH = "Cars/Prefabs/";

    /* these basically get spawned into the world based on templates stored here */
    internal static List<Car> cars;

    internal static void Init()
    {
        cars = new();

        GameObject[] carArray = AssetManager.LoadAssetsInFolder<GameObject>(CAR_PATH);

        foreach (GameObject carObject in carArray)
        {
            Car carPrefab = carObject.GetComponent<Car>();
    
            if (!carPrefab)
            {
                Debug.LogWarning("No car component in car prefab. Adding one...(It will have default settings)");
                carPrefab = carObject.AddComponent<Car>();    
            }

            Car car = MonoBehaviour.Instantiate(carPrefab);

            car.transform.position = new(car.transform.position.x, car.transform.position.y + 1.0f, car.transform.position.z);
            car.configFilePath = CAR_PATH + StringUtils.GetNonCloneName(car.name);
            car.LoadConfig();
            cars.Add(car);
        }

        // in the future we'll have a car selection but just load the first car for now
    }
}