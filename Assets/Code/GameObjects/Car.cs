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

    // This is transient like it is in real life
    float coolness;

    GameObject parent; 

    //
    // METHODS
    //

    private void Start()
    {
        // get the car
        parent = transform.parent.gameObject;

        // find the wheels

        GameObject wheel_left_back = parent.transform.Find("wheel_left_back").gameObject;
        GameObject wheel_left_front = parent.transform.Find("wheel_left_front").gameObject;
        GameObject wheel_right_back = parent.transform.Find("wheel_right_back").gameObject;
        GameObject wheel_right_front = parent.transform.Find("wheel_right_front").gameObject;

        Debug.Assert(wheel_left_back && wheel_left_front && wheel_right_back && wheel_right_front, "Can't find car wheels!");

        // it is possible to use components and create many of these basecars with different gaemassetconfigfile components that have editor-modifiable resource paths
        // BUT IS IT GOOD? Who can know??? We'll have to try it!!
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