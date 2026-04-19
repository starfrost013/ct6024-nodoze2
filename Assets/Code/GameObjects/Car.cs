using System;
using System.Collections.Generic;

using UnityEngine;
using static GameModeRaceMode;

/// <summary>
/// Implements the car
/// </summary>
internal class Car : BasePhysicsObject
{
    //
    // STRUCTS
    // 

    internal struct PhysicsInfo
    {
        // Actual physics data, shared with dominoes
        internal CarModifier data;

        // Characteristics of the car
        internal Int32 steeringRampUpTimer;                         // timer for ramping up steering 

        internal CarSteeringType steeringType;

        // boost state - maybe should become a state enum
        internal bool boosting;                                     // are we boosting?
        internal float boostCurrent;                                // current amount of boost
        internal bool boostEnding;                                  // lets us slowly ramp down
        internal bool inAir;                                        // are we in the air?

        // movement information - now a torque since we use WheelColliders
        internal float forwardTorque;
        internal float rotationTorque;

        internal UInt32 numCollisions;

        internal float fuelCurrent;

        internal WheelCollider wheelLeftBackCollider;
        internal WheelCollider wheelLeftFrontCollider;
        internal WheelCollider wheelRightBackCollider;
        internal WheelCollider wheelRightFrontCollider;
    };

    /// <summary>
    /// User-visible car metadata
    /// </summary>
    internal struct CarMetadata
    {
        /// <summary>
        /// The user-visible name of the car. (renamed to avoid collisions)
        /// </summary>
        internal string friendlyName;

        /// <summary>
        /// A description of the car.
        /// </summary>
        internal string description;

        /// <summary>
        /// The text to show on the handling field of the car seleciton screen.
        /// </summary>
        internal string handlingText;

        /// <summary>
        /// The text to show on the speed field of the car selection screen.
        /// </summary>
        internal string speedText;

        /// <summary>
        /// The text to show on the reliability field of the car selection screen.
        /// </summary>
        internal string reliabilityText;
    };

    internal CarMetadata metadata; 

    //
    // ENUMS
    //

    internal enum CarSteeringType
    {
        Automatic = 0,
        Manual = 1,
    }

    internal enum CarDriveType
    {
        RearWheelDrive = 0,
        FrontWheelDrive = 1,
        FourWheelDrive = 2,
    };

    //
    // FIELDS
    //
    /* The wheels */
    [SerializeField]
    internal GameObject wheelLeftBack;

    [SerializeField]
    internal GameObject wheelLeftFront;

    [SerializeField]
    internal GameObject wheelRightBack;

    [SerializeField]
    internal GameObject wheelRightFront;

    // More configuration data (Serialised fields cannot be located within structs)
    [SerializeField]
    CarSteeringType steeringType;

    [SerializeField]
    CarDriveType driveType;

    /// <summary>
    /// Modifier sets that have been applied 
    /// </summary>
    List<CarModifier> _modifiers = new();

    internal List<CarModifier> modifiers
    {
        get { return _modifiers; }
        private set { _modifiers = value; }
    }

    internal TextAsset configText
    {
        get; private set;
    }

    /// <summary>
    /// Configuration file path
    /// </summary>
    internal string configFilePath;

    //todo: move to "BaseObject" class

    // generic epsilon
    const float EPSILON_MIN = 0.003f;

    /// <summary>
    /// Holds the physics information.
    /// </summary>
    internal PhysicsInfo physics;

    //
    // Misc car-only stuff that the modifiers don't need
    //

    /// <summary>
    /// EXPORT THE DAMN MODEL PROPERLY
    /// </summary>
    private float modelFixCameraAdjY;

    /// <summary>
    /// Percentage of the camera angle. -1 < x < 1, applied as soon as there is an input
    /// </summary>
    float cameraTurnPercentage;

    // car select screen stuff
    /// <summary>
    /// The race has ended, disable the inputs
    /// </summary>
    internal bool disableInputs { get; set; }

    //
    // METHODS
    //

    private void LoadConfigInternal()
    {
        ConfigParser.Parse(configText.text);

        physics.data = new();
        physics.data.Load();

        // additional data needed for cars only
        steeringType = (CarSteeringType)Enum.Parse(typeof(CarSteeringType), ConfigParser.GetValue("Handling", "SteeringType"));
        
        metadata.friendlyName = ConfigParser.GetValue("Info", "Name");
        metadata.description = ConfigParser.GetValue("Info", "Description");
        metadata.handlingText = ConfigParser.GetValue("Info", "HandlingText");
        metadata.speedText = ConfigParser.GetValue("Info", "SpeedText");
        metadata.reliabilityText = ConfigParser.GetValue("Info", "ReliabilityText");

        // If we need X and Z i'm taking matters into my own hands
        float.TryParse(ConfigParser.GetValue("Camera", "ModelFixCameraAdjY"), out modelFixCameraAdjY);
   
        Debug.Assert(configText, "You didn't load a configuration for this car!!!");

        // modifiers are always loaded later 

        // setup stuff not dependent on gameobjects here
        physics.boostCurrent = physics.data.boostMax;
        physics.fuelCurrent = physics.data.fuelMax;
    }

    /// <summary>
    /// Loads the car configuration 
    /// </summary>
    internal void LoadConfig()
    {
        configFilePath = CarManager.CAR_PATH + GameUtils.GetNonCloneName(name);
        configText = AssetManager.LoadAsset<TextAsset>(configFilePath);
        LoadConfigInternal(); 
    }

    /// <summary>
    /// Loads the car configuration from a string so that we don't need to reload it when we spawn prefabs
    /// </summary>
    /// <param name="configStr"></param>
    internal void LoadConfigFromString(string configStr)
    {
        configText = new(configStr);
        LoadConfigInternal(); 
    }

    protected new void Start()
    {
        // prevent crash
        if (!configText)
            return;

        base.Start();
        physRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // find the wheels

        // check the wheels actually have colliders
        physics.wheelLeftBackCollider = wheelLeftBack.GetComponent<WheelCollider>();
        physics.wheelLeftFrontCollider = wheelLeftFront.GetComponent<WheelCollider>();
        physics.wheelRightBackCollider = wheelRightBack.GetComponent<WheelCollider>();
        physics.wheelRightFrontCollider = wheelRightFront.GetComponent<WheelCollider>();

        Debug.Assert(wheelLeftBack && wheelLeftFront && wheelRightBack && wheelRightFront, "Please set the wheels up in the editor!!");
        Debug.Assert(physics.wheelLeftBackCollider && physics.wheelLeftFrontCollider && physics.wheelRightBackCollider && physics.wheelRightFrontCollider, "All car wheels must have WheelColliders!");
    }

    private void CheckAboveKillFloor()
    {
        if (GameManager.GetGameState() != GameManager.GameModeEnum.RaceMode)
            return;

        //TODO: Split gamemoderacemode into gamemoderacemode and GameManager.race ?
        //TODO: Failtypes
                // - Refactor gamestate.afiled to immediately put us in to fail with the known state
                // - Refactor onlegacygui

        GameModeRaceMode raceMode = GameManager.mode as GameModeRaceMode;

        if (transform.position.y < raceMode.raceConfigData.killFloorY)
            raceMode.FailRace(RaceFailReason.OutOfMap, 0);
    }

    private void RunInputFlip()
    {
        bool flipInput = Input.GetKey(KeyCode.F);

        if (!flipInput)
            return;

        transform.localEulerAngles = (new(transform.localEulerAngles.x, transform.localEulerAngles.y, 0.0f));
        transform.position = new(transform.position.x, transform.position.y + 0.2f, transform.position.z);
    }

    private void RunInputMove()
    {
        bool accelerateInput = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);
        bool decelerateInput = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
        bool steerLeftInput = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
        bool steerRightInput = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);

        bool moveInput = (accelerateInput || decelerateInput);
        bool steerInput = (steerLeftInput || steerRightInput);

        // use for fuel checks
        bool anyInput = (moveInput || steerInput);

        if (anyInput)
        {
            // check if we stopped boosting
            bool didWeStopBoosting = physics.boosting;
            physics.boosting = Input.GetKey(KeyCode.LeftShift);
            physics.boostEnding = (didWeStopBoosting && !physics.boosting); // keyup is only true for a single frame and isn't reliable in fixedupdate

            // handle boost shutdown
            if (physics.boosting)
            {
                physics.boostCurrent -= physics.data.boostDepletionPerTick;

                if (physics.boostCurrent < (physics.data.boostMax - physics.data.boostRegenPerTick))
                    physics.boostCurrent += physics.data.boostRegenPerTick;
            }

            // boost isn't finished until we slow down after boost is done
            if (physics.boostEnding
                && (Mathf.Abs(physics.forwardTorque) < physics.data.maxForwardTorque))
            {
                physics.boostEnding = false;
            }

            float forwardAccelHandlingForThisFrame = (physics.inAir) ? physics.data.accelerationForwardAir : physics.data.accelerationForward;
            float steeringAccelHandlingForThisFrame = (physics.inAir) ? physics.data.accelerationSteeringAir : physics.data.accelerationSteering;

            // if we ARE boosting, apply boost accel.
            // if we RECENTLY STOPPED boosting, apply zero accel.
            // otherwise, apply 
            if (physics.boosting)
            {
                forwardAccelHandlingForThisFrame = (physics.inAir) ? physics.data.boostAccelerationForwardAir : physics.data.boostAccelerationForward;
                steeringAccelHandlingForThisFrame = (physics.inAir) ? physics.data.boostAccelerationSteeringAir : physics.data.boostAccelerationSteering;
            }
            else if (physics.boostEnding)
                forwardAccelHandlingForThisFrame = steeringAccelHandlingForThisFrame = 0.0f;  // only apply natural deceleration of boost is ending

            // New code does this calculation automatically - Jan 28, 2025 

            float forwardAccelerationForThisTick = forwardAccelHandlingForThisFrame * Time.fixedDeltaTime;
            float steeringAccelerationForThisTick = steeringAccelHandlingForThisFrame * Time.fixedDeltaTime;

            // if the boost is ending - we want to decelerate
            // we also want to rapidly change direction if we are steering

            if (accelerateInput)
            {
                if (physics.forwardTorque > 0)
                    physics.forwardTorque -= physics.data.decelerationChangeDirection * Time.fixedDeltaTime;

                physics.forwardTorque += -forwardAccelerationForThisTick;
            }

            if (decelerateInput)
            {
                if (physics.forwardTorque < 0)
                    physics.forwardTorque += physics.data.decelerationChangeDirection * Time.fixedDeltaTime;

                physics.forwardTorque += forwardAccelerationForThisTick;
            }

            // multiply so the car can have a smaller turning circle as it gets faster
            float steeringChangeFactor = steeringAccelerationForThisTick;

            if (steerLeftInput
                && (Math.Abs(physics.forwardTorque) > EPSILON_MIN))
            {
                if (physics.rotationTorque > 0)
                    physics.rotationTorque -= physics.data.decelerationChangeDirectionSteering;
                else if (Math.Abs(physics.rotationTorque) < physics.data.maxSteeringTorque)
                    physics.rotationTorque -= steeringChangeFactor; // normalised?

                // camera angle handling
                cameraTurnPercentage -= 0.001f * ((1.0f - 0.001f) * physics.data.cameraTurnFactor);

                if (cameraTurnPercentage < -1.0f)
                    cameraTurnPercentage = -1.0f;
            }

            if (steerRightInput
                && (Math.Abs(physics.forwardTorque) > EPSILON_MIN))
            {
                if (physics.rotationTorque < 0)
                    physics.rotationTorque += physics.data.decelerationChangeDirectionSteering;
                else if (Math.Abs(physics.rotationTorque) < physics.data.maxSteeringTorque)
                    physics.rotationTorque += steeringChangeFactor; // normalised?

                // camera angle handling
                cameraTurnPercentage += 0.001f * ((1.0f - 0.001f) * physics.data.cameraTurnFactor);

                if (cameraTurnPercentage > 1.0f)
                    cameraTurnPercentage = 1.0f;
            }

            // decay camera angle towards zero
            if (!steerLeftInput && !steerRightInput)
            {
                cameraTurnPercentage *= 0.95f;

                if (Math.Abs(cameraTurnPercentage) < float.Epsilon)
                    cameraTurnPercentage = 0;
            }
        }
        
        //
        // APPLICATION OF THE MOMENTUM OF THE CAR
        //

        // this code is awful 

        // apply some natural decay
        if (!anyInput
            || physics.boostEnding) // should we lock this?
        {
            if (physics.inAir)
                physics.forwardTorque *= physics.data.decelerationAir;
            else
                physics.forwardTorque *= physics.data.deceleration;
        }

        if (!steerInput)
            physics.rotationTorque *= physics.data.decelerationSteering;

        float topSpeed = physics.data.maxForwardTorque;

        // test
        // if the player is boosting we don't want them 
        if (physics.boosting || physics.boostEnding)
            topSpeed = physics.data.maxForwardTorqueBoost;

        // anti-big rigs (apply this one at a time)
        if (physics.forwardTorque > topSpeed)
            physics.forwardTorque = topSpeed;
        else if (physics.forwardTorque < -topSpeed)
            physics.forwardTorque = -topSpeed;

        //
        // APPLY MOTION
        //

        if (physRigidbody.linearVelocity.magnitude < physics.data.maxVelocity)
        {
            bool needFrontWheelDrive = (driveType == (CarDriveType.FrontWheelDrive) || (driveType == (CarDriveType.FourWheelDrive)));
            bool needRearWheelDrive = (driveType == (CarDriveType.RearWheelDrive) || (driveType == (CarDriveType.FourWheelDrive)));

            if (needFrontWheelDrive)
            {
                physics.wheelLeftFrontCollider.motorTorque = (moveInput) ? (physics.wheelLeftFrontCollider.motorTorque + physics.forwardTorque) : 0;
                physics.wheelRightFrontCollider.motorTorque = (moveInput) ? (physics.wheelRightFrontCollider.motorTorque + physics.forwardTorque) : 0;
            }

            if (needRearWheelDrive)
            {
                physics.wheelLeftBackCollider.motorTorque = (moveInput) ? (physics.wheelLeftBackCollider.motorTorque + physics.forwardTorque) : 0;
                physics.wheelRightBackCollider.motorTorque = (moveInput) ? (physics.wheelRightBackCollider.motorTorque + physics.forwardTorque) : 0;
            }
        }

        // car rotation 

        // apply a final factor baseed on the linear velocity
        float steeringRampFactor = Math.Clamp(physRigidbody.linearVelocity.magnitude / physics.data.maxSteeringVelocity, 
            physics.data.minSteeringAmount, physics.data.maxSteeringAmount);

        transform.localEulerAngles = new Vector3(
            transform.localEulerAngles.x,
            // always divide by 60 as fixedupdate updates 60 times per second
            transform.localEulerAngles.y + (physics.rotationTorque / 20.0f) * steeringRampFactor * 360.0f * Time.fixedDeltaTime,
            transform.localEulerAngles.z);

        // rotate the wheels (todo: move3 everything into an array)
        // setting position has some unfortunate consequences
        physics.wheelLeftBackCollider.GetWorldPose(out Vector3 _, out Quaternion wheelRotation);
        wheelLeftBack.transform.rotation = wheelRotation;
        physics.wheelLeftFrontCollider.GetWorldPose(out Vector3 _, out wheelRotation);
        wheelLeftFront.transform.rotation = wheelRotation;
        physics.wheelRightBackCollider.GetWorldPose(out Vector3 _, out wheelRotation);
        wheelRightBack.transform.rotation = wheelRotation;
        physics.wheelRightFrontCollider.GetWorldPose(out Vector3 _, out wheelRotation);
        wheelRightFront.transform.rotation = wheelRotation;

        //
        // Fuel handling 
        //

        float fuelUseMultiplier = Math.Clamp(physRigidbody.linearVelocity.magnitude / physics.data.maxFuelUseVelocity,
        physics.data.minFuelUseAmount, physics.data.maxFuelUseAmount);

        Debug.Log("Fuel Use Multiplier: " + fuelUseMultiplier);

        if (physics.data.decelerationFuelCutoff
            && !anyInput)
        {
            fuelUseMultiplier = 0f;
        }

        physics.fuelCurrent -= (physics.data.fuelDepletionPerTick * fuelUseMultiplier);
    }

    private void UpdateCamera()
    {
        // car was incorrectly exported and bad bad artists won't re-export
        // Fix when model correctly imported
        Vector3 carRot = transform.rotation.eulerAngles;
        float newEulerY = ((carRot.y + modelFixCameraAdjY) % 360) + (physics.data.maxTurnCameraAngle * cameraTurnPercentage);

        Camera.main.transform.localEulerAngles = new Vector3(Camera.main.transform.localEulerAngles.x,
                newEulerY,
                Camera.main.transform.localEulerAngles.z
                );

        /* also move a bit forward depending on our overall speed */
        Camera.main.transform.position = transform.position + (transform.forward * physics.data.cameraRelativeZ);
        /* Dumb ass way of doing it - there's a better way. */
        Camera.main.transform.position += physics.data.cameraRelativeX * (transform.right * cameraTurnPercentage);
        Camera.main.transform.position += new Vector3(0.0f, physics.data.cameraRelativeY, 0.0f);
    }

    // FixedUpdate contains our controls so they feel decent regardless of fraemrate
    // I don't have time to use ISP sorry!
    private void FixedUpdate()
    {
        // Check if the race is active so we can move (using temporary UI)
        GameMode mode = GameManager.mode;
        GameModeRaceMode raceMode = (GameModeRaceMode)mode;

        // HACK (avoids us doing a second call
        if (GameManager.GetGameState() == GameManager.GameModeEnum.RaceMode)
        {

            if (raceMode.raceState != GameModeRaceMode.RaceState.Active)
                return;
        }

        if (physics.fuelCurrent <= 0)
        {
            physics.fuelCurrent = 0;
            raceMode.FailRace(RaceFailReason.OutOfFuel); // we still need to split this out
            disableInputs = true; 
        }

        CheckAboveKillFloor();

        // run code to check all our inputs

        if (!disableInputs)
        {
            RunInputFlip(); // this is separate so make it its own tihng
            RunInputMove(); // the main input stuff
        }

        //always update the camera
        UpdateCamera();
    }

    /// <summary>
    /// Called by dominoes when they want to apply a modifier to the car so that all updates can be done at once
    /// </summary>
    /// <returns></returns>
    internal CarModifier GetCarModifiers()
    {
        return physics.data; 
    }

    /// <summary>
    /// Called once all physics modifiers have been applied.
    /// </summary>
    /// <param name="info"></param>
    internal void ApplyModifierSet(CarModifier info)
    {
        // is this needed? probably needed later
        modifiers.Add(info);

        // The worst code ever
        // TODO: turn into an operator...
        physics.data.maxVelocity += info.maxVelocity;
        physics.data.accelerationForward += info.accelerationForward;
        physics.data.accelerationForwardAir += info.accelerationForwardAir;
        physics.data.accelerationSteering += info.accelerationSteering;
        physics.data.accelerationSteeringAir += info.accelerationSteeringAir;
        physics.data.boostAccelerationForward += info.boostAccelerationForward;
        physics.data.boostAccelerationForwardAir += info.boostAccelerationForwardAir;
        physics.data.boostAccelerationSteering += info.boostAccelerationSteering;
        physics.data.boostAccelerationSteeringAir += info.boostAccelerationSteeringAir;
        physics.data.boostMax += info.boostMax;
        physics.data.deceleration += info.deceleration;
        physics.data.decelerationAir += info.decelerationAir;
        physics.data.decelerationChangeDirection += info.decelerationChangeDirection;
        physics.data.decelerationChangeDirectionSteering += info.decelerationChangeDirectionSteering;
        physics.data.decelerationSteering += info.decelerationSteering;
        physics.data.maxSteeringTorque += info.maxSteeringTorque;
        physics.data.minSteeringAmount += info.minSteeringAmount;
        physics.data.maxSteeringAmount += info.maxSteeringAmount;   
        physics.data.maxSteeringVelocity += info.maxSteeringVelocity;
        physics.data.maxForwardTorque += info.maxForwardTorque;
        physics.data.maxForwardTorqueBoost += info.maxForwardTorqueBoost;

        physics.data.maxTurnCameraAngle += info.maxTurnCameraAngle;
        physics.data.maxTurnCameraAmount += info.maxTurnCameraAmount;
        physics.data.cameraRelativeX += info.cameraRelativeX;
        physics.data.cameraRelativeY += info.cameraRelativeY;
        physics.data.cameraRelativeZ += info.cameraRelativeZ;
        physics.data.cameraTurnFactor += info.cameraTurnFactor;

        physics.data.fuelMax += info.fuelMax;   
        physics.data.fuelDepletionPerTick += info.fuelDepletionPerTick;
        physics.data.decelerationFuelCutoff = info.decelerationFuelCutoff; 
        physics.data.refuelGaragePercent += info.refuelGaragePercent;
        physics.data.minFuelUseAmount += info.minFuelUseAmount;
        physics.data.maxFuelUseAmount += info.maxFuelUseAmount;
        physics.data.maxFuelUseVelocity += info.maxFuelUseVelocity;
    }

    /// <summary>
    /// Returns true if the car has the modifier set "name"
    /// </summary>
    /// <param name="name">The internal name (file name) modifier set to look for</param>
    /// <returns>A boolean indicating if the car has the modifier set set.</returns>
    internal bool HasModifierSet(string name)
    {
        foreach (CarModifier modifier in modifiers)
        {
            if (modifier.internalModifierName == name)
                return true;
        }

        return false; 
    }

    /// <summary>
    /// Returns true if the car has the modifier set with the user visible name "name"
    /// </summary>
    /// <param name="name">The internal name (file name) modifier set to look for</param>
    /// <returns>A boolean indicating if the car has the modifier set set.</returns>
    internal bool HasModifierSetUserVisibleName(string name)
    {
        foreach (CarModifier modifier in modifiers)
        {
            if (modifier.modifierName == name)
                return true;
        }

        return false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        physics.numCollisions++;    
    }

    private void OnCollisionExit(Collision collision)
    {
        physics.numCollisions--;
        physics.inAir = (physics.numCollisions == 0);
        //if (physics.inAir)
           //Debug.Log("In Air");
    }
}