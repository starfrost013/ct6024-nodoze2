
/*
 * Modifiers that can be applied to cars by dominoes or anything else.
 * 
 * For cars, these are ABSOLUTE values.
 * For dominoes, these are RELATIVE values (i.e. -0.5 to remove 0.5 from it)
 */
using System;
using Unity.VisualScripting;
using UnityEngine;

internal class CarModifier
{
    // Characteristics of the car
    internal float maxForwardTorque;
    internal float maxForwardTorqueBoost;
    internal float accelerationForward;                         // acceleration while moving forward
    internal float accelerationSteering;
    internal float accelerationForwardAir;
    internal float accelerationSteeringAir;
    internal float deceleration;
    internal float decelerationSteering;                        // deceleration while steering
    internal float decelerationAir;                             // deceleration in the air
    internal float decelerationChangeDirection;                 // deceleration when the car changes direction [W/S]
    internal float decelerationChangeDirectionSteering;         // deceleration when the car changes direction [A/D]
    internal float maxSteeringTorque;                           // maximum torque
    internal float minSteeringAmount;                           // minimum speed steering magnitude at 0 speed while stopped
    internal float maxSteeringAmount;                           // maximum speed steering magnitude at maxSteeringVelocity
    internal float maxSteeringVelocity;                         // maximum velocity magnitude

    // Boosting characteristics of the car
    internal float boostMax;                                    // total boost amount per 1/60 of a second
    internal float boostDepletionPerTick;                       // total boost depletion per 1/60 of a second
    internal float boostRegenPerTick;                           // total boost regen per 1/60 of a second
    internal float boostAccelerationForward;
    internal float boostAccelerationSteering;
    internal float boostAccelerationForwardAir;
    internal float boostAccelerationSteeringAir;

    // Camera characteristics of the car
    internal float maximumTurnCameraAngle;
    internal float maximumTurnCameraAmount;
    internal float cameraRelativeX;                             // relative camera X coord to car
    internal float cameraRelativeY;                             // relative camera Y coord to car
    internal float cameraRelativeZ;                             // relative camera Z coord to car
    internal float cameraTurnFactor;                            // turn factor of the camera
    
    // Fueling characteristics of the car
    internal float fuelMax;                                     // maximum fuel amount
    internal float fuelDepletionPerTick;
    internal float refuelGaragePercent;

    internal bool decelerationFuelCutoff;

    // renamed to avoid confusion with car names e.g. car select screen
    internal string modifierName;
    internal string internalModifierName;                       // same as domino.internalName

    internal void Load()
    {
        // this is horrible but seemed to be the best way to determine if at least one parse failed
        bool success = float.TryParse(ConfigParser.GetValue("Handling", "MaxForwardTorque"), out maxForwardTorque)
        | float.TryParse(ConfigParser.GetValue("Handling", "MaxForwardTorqueBoost"), out maxForwardTorqueBoost)
        | float.TryParse(ConfigParser.GetValue("Handling", "MaxSteeringTorque"), out maxSteeringTorque)
        | float.TryParse(ConfigParser.GetValue("Handling", "AccelerationForward"), out accelerationForward)
        | float.TryParse(ConfigParser.GetValue("Handling", "AccelerationSteering"), out accelerationSteering)
        | float.TryParse(ConfigParser.GetValue("Handling", "AccelerationForwardAir"), out accelerationForwardAir)
        | float.TryParse(ConfigParser.GetValue("Handling", "AccelerationSteeringAir"), out accelerationSteeringAir)
        | float.TryParse(ConfigParser.GetValue("Handling", "Deceleration"), out deceleration)
        | float.TryParse(ConfigParser.GetValue("Handling", "DecelerationSteering"), out decelerationSteering)
        | float.TryParse(ConfigParser.GetValue("Handling", "DecelerationAir"), out decelerationAir)
        | float.TryParse(ConfigParser.GetValue("Handling", "DecelerationChangeDirection"), out decelerationChangeDirection)
        | float.TryParse(ConfigParser.GetValue("Handling", "DecelerationChangeDirectionSteering"), out decelerationChangeDirectionSteering)
        | float.TryParse(ConfigParser.GetValue("Handling", "MinimumSteeringAmount"), out minSteeringAmount)
        | float.TryParse(ConfigParser.GetValue("Handling", "MaximumSteeringAmount"), out maxSteeringAmount)
        | float.TryParse(ConfigParser.GetValue("Handling", "MaximumSteeringVelocity"), out maxSteeringVelocity)
        | float.TryParse(ConfigParser.GetValue("Handling", "BoostAmount"), out boostMax)
        | float.TryParse(ConfigParser.GetValue("Handling", "BoostDepletionPerTick"), out boostDepletionPerTick)
        | float.TryParse(ConfigParser.GetValue("Handling", "BoostAccelerationForward"), out boostAccelerationForward)
        | float.TryParse(ConfigParser.GetValue("Handling", "BoostAccelerationSteering"), out boostAccelerationSteering)
        | float.TryParse(ConfigParser.GetValue("Handling", "BoostAccelerationForwardAir"), out boostAccelerationForwardAir)
        | float.TryParse(ConfigParser.GetValue("Handling", "BoostAccelerationSteeringAir"), out boostAccelerationSteeringAir)
        | float.TryParse(ConfigParser.GetValue("Camera", "MaximumTurnCameraAngle"), out maximumTurnCameraAngle)
        | float.TryParse(ConfigParser.GetValue("Camera", "MaximumTurnCameraAmount"), out maximumTurnCameraAmount)
        | float.TryParse(ConfigParser.GetValue("Camera", "CameraRelativeX"), out cameraRelativeX)
        | float.TryParse(ConfigParser.GetValue("Camera", "CameraRelativeY"), out cameraRelativeY)
        | float.TryParse(ConfigParser.GetValue("Camera", "CameraRelativeZ"), out cameraRelativeZ)
        | float.TryParse(ConfigParser.GetValue("Camera", "CameraTurnFactor"), out cameraRelativeZ)
        | float.TryParse(ConfigParser.GetValue("Fuel", "Max"), out fuelMax)
        | float.TryParse(ConfigParser.GetValue("Fuel", "DepletionPerTick"), out fuelDepletionPerTick)
        | float.TryParse(ConfigParser.GetValue("Fuel", "RefuelGaragePercent"), out refuelGaragePercent)
        | bool.TryParse(ConfigParser.GetValue("Fuel", "DecelerationFuelCutoff"), out decelerationFuelCutoff);
        ;
        
        if (!success)
            Debug.LogWarning("Some car modifiers failed to load. This may be intended or not...");

        return;
    
    }
}