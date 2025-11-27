
using UnityEngine;

// It's a car :D

internal class Car : MonoBehaviour
{
    GameAssetConfigFile spec;

    // Characteristics of the car
    float topSpeed;
    float acceleration;

    enum CarSteeringType
    {
        Automatic = 0,
        Manual = 1,
    }

    CarSteeringType type;

    float boostAmount;
    float boostAccelerator;

    private void Start()
    {
        spec = (GameAssetConfigFile)AssetManager.LoadAsset<GameAssetConfigFile>("/Data/Cars/CarTest.cfg");
        spec.ParseCfg();
    }

    private void Update()
    {
        
    }
}