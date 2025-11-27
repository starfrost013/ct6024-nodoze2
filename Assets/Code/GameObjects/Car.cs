using System;
using UnityEngine;

// It's a car :D
internal class Car : MonoBehaviour
{
    //
    // STRUCTS
    // 

    enum CarSteeringType
    {
        Automatic = 0,
        Manual = 1,
    }

    //
    // FIELDS
    //

    GameAssetConfigFile spec;

    // Characteristics of the car
    float topSpeed;
    float topSpeedBoost;
    float acceleration;

    CarSteeringType steeringType;

    float boostAmount;
    float boostAcceleration;

    float coolness;

    //
    // METHODS
    //

    private void Start()
    {
        spec = (GameAssetConfigFile)AssetManager.LoadAsset<GameAssetConfigFile>("/Cars/CarTest.cfg"); // in the future cars will overload from this
        spec.ParseCfg();

        topSpeed = float.Parse(spec.GetValue("Handling", "TopSpeed"));
        topSpeedBoost = float.Parse(spec.GetValue("Handling", "TopSpeedBoost"));
        acceleration = float.Parse(spec.GetValue("Handling", "Acceleration"));
        boostAmount = float.Parse(spec.GetValue("Handling", "BoostAmount"));
        boostAcceleration = float.Parse(spec.GetValue("Handling", "BoostAcceleration"));
        steeringType = (CarSteeringType)Enum.Parse(typeof(CarSteeringType), spec.GetValue("Handling", "SteeringType"));
    }

    private void Update()
    {
        
    }
}