using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

//
// Entrance point for car system. Called from GameModeInit
//
internal static class CarManager
{
    // change this when we have assetbundles
    internal const string CAR_PATH = "Cars/";

    /* these basically get spawned into the world based on prototype objects stored here */
    internal static GameObject[] carArray;

    private static bool initialised;

    internal static void Init()
    {
        // don't reinit on e.g. race restart
        if (!initialised)
            carArray = AssetManager.LoadAssetsInFolder<GameObject>(CAR_PATH);

        initialised = true;

        // in the future we'll have a car selection but just load the first car for now
    }

    internal static Car GetCarPrefabByName(string name)
    {
        foreach (GameObject carObject in carArray)
        {
            Car carPrefab = carObject.GetComponent<Car>();

            if (!carPrefab)
            {
                Debug.LogWarning("No car component in car prefab. Adding one...(It will have default settings)");
                carPrefab = carObject.AddComponent<Car>();
            }

            if (string.Equals(carPrefab.name, name))
                return carPrefab;
        }

        Debug.LogWarning("Failed to find the car by the name " + name + " Make sure the car is named correctly in the inspector.");

        return null;
    }

    /// <summary>
    /// Set the player car
    /// </summary>
    /// <param name="name">The name ofthe car in the car prefabs folder to load</param>
    internal static void SetPlayerCar(string name)
    {
        Car carPrefab = GetCarPrefabByName(name);

        if (carPrefab != null)
        {
            if (GameManager.player.HasCar())
            {
                // get rid of the car that already exists
                GameManager.player.DestroyCar();
            }

            GameManager.player.car = MonoBehaviour.Instantiate(carPrefab);

            // move the car to the start location
            GameObject start = GameObject.Find("CarStart");

            if (start != null)
                GameManager.player.car.transform.position = start.transform.position + new Vector3(0.5f, 1.0f, 0.5f);
            else
            {
                Debug.LogWarning("Please insert a start point!");

                // ensure it's not stuck
                GameManager.player.car.transform.position = new(GameManager.player.car.transform.position.x, 
                    GameManager.player.car.transform.position.y + 1.0f, 
                    GameManager.player.car.transform.position.z);
            }

            GameManager.player.car.LoadConfig();
        }
    }
 
    /// <summary>
    /// Apply a modifier set to the player's car
    /// </summary>
    /// <param name="domino">Domino to apply to the player car</param>
    internal static void ApplyModifierSetToPlayerCar(CarModifier modifier)
    {
        GameManager.player.car.ApplyModifierSet(modifier);
    }
}