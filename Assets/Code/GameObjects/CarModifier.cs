
/*
 * Modifiers that can be applied to cars by dominoes or anything else.
 * 
 * For cars, these are ABSOLUTE values.
 * For dominoes, these are RELATIVE values (i.e. -0.5 to remove 0.5 from it)
 */
using System;
using UnityEngine;

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
        bool success = false;

        // this is horrible but seemed to be the best way to determine if at least one parse failed
        success = float.TryParse(ConfigParser.GetValue("Handling", "TopSpeed"), out topSpeed)
        | float.TryParse(ConfigParser.GetValue("Handling", "TopSpeedBoost"), out topSpeedBoost)
        | float.TryParse(ConfigParser.GetValue("Handling", "AccelerationForward"), out accelerationForward)
        | float.TryParse(ConfigParser.GetValue("Handling", "AccelerationSteering"), out accelerationSteering)
        | float.TryParse(ConfigParser.GetValue("Handling", "AccelerationForwardAir"), out accelerationForwardAir)
        | float.TryParse(ConfigParser.GetValue("Handling", "AccelerationSteeringAir"), out accelerationSteeringAir)
        | float.TryParse(ConfigParser.GetValue("Handling", "Deceleration"), out deceleration)
        | float.TryParse(ConfigParser.GetValue("Handling", "DecelerationSteering"), out decelerationSteering)
        | float.TryParse(ConfigParser.GetValue("Handling", "DecelerationAir"), out decelerationAir)
        | float.TryParse(ConfigParser.GetValue("Handling", "DecelerationChangeDirection"), out decelerationChangeDirection)
        | float.TryParse(ConfigParser.GetValue("Handling", "DecelerationChangeDirectionSteering"), out decelerationChangeDirectionSteering)
        | float.TryParse(ConfigParser.GetValue("Handling", "MaxSteeringTorque"), out maxSteeringTorque)
        | float.TryParse(ConfigParser.GetValue("Handling", "SteeringRampUpTicks"), out steeringRampUpTicks)
        | float.TryParse(ConfigParser.GetValue("Handling", "BoostAmount"), out boostAmount)
        | float.TryParse(ConfigParser.GetValue("Handling", "BoostAccelerationForward"), out boostAccelerationForward)
        | float.TryParse(ConfigParser.GetValue("Handling", "BoostAccelerationSteering"), out boostAccelerationSteering)
        | float.TryParse(ConfigParser.GetValue("Handling", "BoostAccelerationForwardAir"), out boostAccelerationForwardAir)
        | float.TryParse(ConfigParser.GetValue("Handling", "BoostAccelerationSteeringAir"), out boostAccelerationSteeringAir);
        
        if (!success)
            Debug.LogWarning("Some car modifiers failed to load. This may be intended or not...");

        return;
    // we need to check the condition above a lot of times, so to avoid duplicating failure code, we use a GOTO
    
    }
}