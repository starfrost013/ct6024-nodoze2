
/*
 * Modifiers that can be applied to cars by dominoes or anything else.
 * 
 * For cars, these are ABSOLUTE values.
 * For dominoes, these are RELATIVE values (i.e. -0.5 to remove 0.5 from it)
 */
using System;
using UnityEngine;
using static Car;

internal class CarModifier
{
    // Characteristics of the car
    internal float topSpeed;
    internal float topSpeedBoost;
    internal float accelerationForward;                         // acceleration while steering
    internal float accelerationSteering;
    internal float accelerationForwardAir;
    internal float accelerationSteeringAir;
    internal float deceleration;
    internal float decelerationSteering;                        // deceleration while steering
    internal float decelerationAir;                             // deceleration in the air
    internal float decelerationChangeDirection;                 // deceleration when the car changes direction [W/S]
    internal float decelerationChangeDirectionSteering;         // deceleration when the car changes direction [A/D]
    internal float maxSteeringTorque;                           // maximum torque
    internal float steeringRampUpTicks;                         // timer for ramping up steering 

    internal float steeringIntensity;                           // the intensity of the steering

    // Boosting characteristics of the car
    internal float boostAmount;
    internal float boostAccelerationForward;
    internal float boostAccelerationSteering;
    internal float boostAccelerationForwardAir;
    internal float boostAccelerationSteeringAir;

    internal void Load()
    {

        try
        {
            topSpeed = float.Parse(ConfigParser.GetValue("Handling", "TopSpeed"));
            topSpeedBoost = float.Parse(ConfigParser.GetValue("Handling", "TopSpeedBoost"));
            accelerationForward = float.Parse(ConfigParser.GetValue("Handling", "AccelerationForward"));
            accelerationSteering = float.Parse(ConfigParser.GetValue("Handling", "AccelerationSteering"));
            accelerationForwardAir = float.Parse(ConfigParser.GetValue("Handling", "AccelerationForwardAir"));
            accelerationSteeringAir = float.Parse(ConfigParser.GetValue("Handling", "AccelerationSteeringAir"));

            deceleration = float.Parse(ConfigParser.GetValue("Handling", "Deceleration"));
            decelerationSteering = float.Parse(ConfigParser.GetValue("Handling", "DecelerationSteering"));
            decelerationAir = float.Parse(ConfigParser.GetValue("Handling", "DecelerationAir"));
            decelerationChangeDirection = float.Parse(ConfigParser.GetValue("Handling", "DecelerationChangeDirection"));
            decelerationChangeDirectionSteering = float.Parse(ConfigParser.GetValue("Handling", "DecelerationChangeDirectionSteering"));
            maxSteeringTorque = float.Parse(ConfigParser.GetValue("Handling", "MaxSteeringTorque"));
            steeringRampUpTicks = float.Parse(ConfigParser.GetValue("Handling", "SteeringRampUpTicks"));

            boostAmount = float.Parse(ConfigParser.GetValue("Handling", "BoostAmount"));
            boostAccelerationForward = float.Parse(ConfigParser.GetValue("Handling", "BoostAccelerationForward"));
            boostAccelerationSteering = float.Parse(ConfigParser.GetValue("Handling", "BoostAccelerationSteering"));
            boostAccelerationForwardAir = float.Parse(ConfigParser.GetValue("Handling", "BoostAccelerationForwardAir"));
            boostAccelerationSteeringAir = float.Parse(ConfigParser.GetValue("Handling", "BoostAccelerationSteeringAir"));

        }
        catch (Exception e)
        {
            Debug.LogError("FAILED to load car settings!!!: " + e);
        }
    }
}