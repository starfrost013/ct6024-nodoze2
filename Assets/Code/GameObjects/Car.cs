using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

// It's a car :D
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
        internal bool boosting;
        internal bool boostEnding;                        // lets us slowly ramp down
        internal bool inAir;

        // movement information - now a torque since we use WheelColliders
        internal float forwardTorque;
        internal float rotationTorque;

        internal UInt32 numCollisions;

        internal WheelCollider wheelLeftBackCollider;
        internal WheelCollider wheelLeftFrontCollider; 
        internal WheelCollider wheelRightBackCollider;
        internal WheelCollider wheelRightFrontCollider;
    };

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
    List<CarModifier> appliedModifierSets;

    TextAsset configText;

    /// <summary>
    /// Configuration file path
    /// </summary>
    internal string configFilePath;
  
    // This is transient like it is in real life
    float coolness;

    //todo: move to "BaseObject" class

    // generic epsilon
    const float EPSILON_MIN = 0.003f;

    // physics information
    PhysicsInfo physics;

    //
    // METHODS
    //

    /* Loads the configuration */
    internal void LoadConfig()
    {
        configText = AssetManager.LoadAsset<TextAsset>(configFilePath);
        ConfigParser.Parse(configText.text);

        physics.data = new();
        physics.data.Load();

        // additional data needed for cars only
        steeringType = (CarSteeringType)Enum.Parse(typeof(CarSteeringType), ConfigParser.GetValue("Handling", "SteeringType"));

        Debug.Assert(configText, "You didn't load a configuration for this car!!!");
    }

    protected new void Start()
    {
        // prevent crash
        if (!configText)
            return;

        base.Start();
        physRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // find the wheels

        Debug.Assert(wheelLeftBack && wheelLeftFront && wheelRightBack && wheelRightFront, "Please set the wheels up in the editor!!");

        // check the wheels actually have colliders
        physics.wheelLeftBackCollider = wheelLeftBack.GetComponent<WheelCollider>();
        physics.wheelLeftFrontCollider = wheelLeftFront.GetComponent<WheelCollider>();
        physics.wheelRightBackCollider = wheelRightBack.GetComponent<WheelCollider>();
        physics.wheelRightFrontCollider = wheelRightFront.GetComponent<WheelCollider>();

        Debug.Assert(physics.wheelLeftBackCollider && physics.wheelLeftFrontCollider && physics.wheelRightBackCollider && physics.wheelRightFrontCollider, "All car wheels must have WheelColliders!");
    }

    // FixedUpdate contains our controls so they feel decent regardless of fraemrate
    // I don't have time to use ISP sorry!
    private void FixedUpdate()
    {
        // Check if the race is active so we can move (using temporary UI)
        GameMode mode = GameManager.GetGameModeObject();

        // HACK (avoids us doing a second call
        if (mode is GameModeRaceMode)
        {
            GameModeRaceMode raceMode = (GameModeRaceMode)mode;

            if (raceMode.raceState != GameModeRaceMode.RaceState.Active)
                return;
        }

        // Start by reading inputs 

        bool accelerateInput = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);
        bool decelerateInput = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
        bool steerLeftInput = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
        bool steerRightInput = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);
        
        bool moveInput = (accelerateInput || decelerateInput);
        bool steerInput = (steerLeftInput || steerRightInput);   

        // check if we stopped boosting
        physics.boosting = Input.GetKey(KeyCode.LeftShift);
        physics.boostEnding = Input.GetKeyUp(KeyCode.LeftShift);

        // boost isn't finished until we slow down after boost is done
        if (physics.boostEnding
            && (Mathf.Abs(physics.forwardTorque) < physics.data.topSpeed))
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

        float forwardAccelerationForThisFrame = forwardAccelHandlingForThisFrame * Time.fixedDeltaTime;
        float steeringAccelerationForThisFrame = steeringAccelHandlingForThisFrame * Time.fixedDeltaTime;

        // if the boost is ending - we want to decelerate
        // we also want to rapidly change direction if we are steering

        if (accelerateInput)
        {
            if (physics.forwardTorque > 0)
                physics.forwardTorque -= physics.data.decelerationChangeDirection;

            physics.forwardTorque += -forwardAccelerationForThisFrame;
        }

        if (decelerateInput)
        {
            if (physics.forwardTorque < 0)
                physics.forwardTorque += physics.data.decelerationChangeDirection;

            physics.forwardTorque += forwardAccelerationForThisFrame;
        }

        // multiply so the car can have a smaller turning circle as it gets faster
        float steeringChangeFactor = steeringAccelerationForThisFrame;

        if (Math.Abs(physics.forwardTorque) > 1.0f)
            steeringChangeFactor *= Math.Abs(physics.forwardTorque) / 3.0f;

        if (steerLeftInput
            && (Math.Abs(physics.forwardTorque) > EPSILON_MIN))
        {
            if (physics.rotationTorque > 0)
                physics.rotationTorque -= physics.data.decelerationChangeDirectionSteering;
            else if (Math.Abs(physics.rotationTorque) < physics.data.maxSteeringTorque)
                physics.rotationTorque -= steeringChangeFactor; // normalised?
        }

        if (steerRightInput
            && (Math.Abs(physics.forwardTorque) > EPSILON_MIN))
        {
            if (physics.rotationTorque < 0)
                physics.rotationTorque += physics.data.decelerationChangeDirectionSteering;
            else if (Math.Abs(physics.rotationTorque) < physics.data.maxSteeringTorque)
                physics.rotationTorque += steeringChangeFactor; // normalised?
        }


        //
        // APPLICATION OF THE MOMENTUM OF THE CAR
        //

        // this code is awful 

        // apply some natural decay
        if ((!moveInput && !steerInput)
            || physics.boostEnding) // should we lock this?
        {
            if (physics.inAir)
                physics.forwardTorque *= physics.data.decelerationAir;
            else
                physics.forwardTorque *= physics.data.deceleration;
        }

        if (!steerInput)
            physics.rotationTorque *= physics.data.decelerationSteering;

        // Debug.Log("Velocity: " + physics.velocity.x + " " + physics.velocity.y + " " + physics.velocity.z);

        float topSpeed = physics.data.topSpeed;

        // test
        // if the player is boosting we don't want them 
        if (physics.boosting || physics.boostEnding)
            topSpeed = physics.data.topSpeedBoost;

        // anti-big rigs (apply this one at a time)
        if (physics.forwardTorque > topSpeed)
            physics.forwardTorque = topSpeed;
        else if (physics.forwardTorque < -topSpeed)
            physics.forwardTorque = -topSpeed;

        bool needFrontWheelDrive = (driveType == (CarDriveType.FrontWheelDrive) || (driveType == (CarDriveType.FourWheelDrive)));
        bool needRearWheelDrive = (driveType == (CarDriveType.RearWheelDrive) || (driveType == (CarDriveType.FourWheelDrive)));

        if (needFrontWheelDrive)
        {
            // we want to store the current velocity separately to the actual torque
            if (!moveInput)
            {
                physics.wheelLeftFrontCollider.motorTorque = 0;
                physics.wheelRightFrontCollider.motorTorque = 0;
            }
            else
            {
                physics.wheelLeftFrontCollider.motorTorque += physics.forwardTorque;
                physics.wheelRightFrontCollider.motorTorque += physics.forwardTorque;
            }

            // TODO: rotate the wheels
        }

        if (needRearWheelDrive)
        {
            // we want to store the current velocity separately to the actual torque
            if (!moveInput)
            {
                physics.wheelLeftBackCollider.motorTorque = 0;
                physics.wheelRightBackCollider.motorTorque = 0;
            }
            else
            {
                physics.wheelLeftBackCollider.motorTorque += physics.forwardTorque;
                physics.wheelRightBackCollider.motorTorque += physics.forwardTorque;
            }
        }

        // car rotation 

        transform.localEulerAngles = new Vector3(
            transform.localEulerAngles.x,
            // always divide by 60 as fixedupdate updates 60 times per second
            transform.localEulerAngles.y + (physics.rotationTorque / 60.0f) * 360.0f * Time.fixedDeltaTime,
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

        // apply motion

        // car was incorrectly exported and bad bad artists won't re-export
        // Fix when model correctly imported
        Vector3 carRot = transform.rotation.eulerAngles;
        float newEulerY = (carRot.y + 180.0f) % 360;
        newEulerY += 19.0f * (physics.rotationTorque / physics.data.maxSteeringTorque);

        Camera.main.transform.localEulerAngles = new Vector3(Camera.main.transform.localEulerAngles.x,
            newEulerY,
            Camera.main.transform.localEulerAngles.z
            );
        
        /* also move a bit forward depending on our overall speed */ 
        Camera.main.transform.position = transform.position + (transform.forward * 5.0f);
        /* Dumb ass way of doing it - there's a better way. */
        Camera.main.transform.position += 0.1f * ((transform.right * physics.rotationTorque) * (physics.forwardTorque / physics.data.topSpeed));
        Camera.main.transform.position += new Vector3(0.0f, 1.4f, 0.0f);
    }

    /// <summary>
    /// Called by dominoes when they want to apply a modifier to the car so that all updates can be done at once
    /// </summary>
    /// <returns></returns>
    internal CarModifier GetCarModifier()
    {
        return physics.data; 
    }

    /// <summary>
    /// Called once all physics modifiers have been applied.
    /// </summary>
    /// <param name="info"></param>
    internal void ApplyModifier(CarModifier info)
    {
        // is this needed? probably needed later
        appliedModifierSets.Add(info);

        // The worst code ever
        physics.data.accelerationForward += info.accelerationForward;
        physics.data.accelerationForwardAir += info.accelerationForwardAir;
        physics.data.accelerationSteering += info.accelerationSteering;
        physics.data.accelerationSteeringAir += info.accelerationSteeringAir;
        physics.data.boostAccelerationForward += info.boostAccelerationForward;
        physics.data.boostAccelerationForwardAir += info.boostAccelerationForwardAir;
        physics.data.boostAccelerationSteering += info.boostAccelerationSteering;
        physics.data.boostAccelerationSteeringAir += info.boostAccelerationSteeringAir;
        physics.data.boostAmount += info.boostAmount;
        physics.data.deceleration += info.deceleration;
        physics.data.decelerationAir += info.decelerationAir;
        physics.data.decelerationChangeDirection += info.decelerationChangeDirection;
        physics.data.decelerationChangeDirectionSteering += info.decelerationChangeDirectionSteering;
        physics.data.decelerationSteering += info.decelerationSteering;
        physics.data.maxSteeringTorque += info.maxSteeringTorque;
        physics.data.steeringRampUpTicks += info.steeringRampUpTicks;
        physics.data.topSpeed += info.topSpeed;
        physics.data.topSpeedBoost += info.topSpeedBoost;
    }

    private void OnCollisionEnter(Collision collision)
    {
        physics.numCollisions++;    
    }

    private void OnCollisionExit(Collision collision)
    {
        physics.numCollisions--;

        physics.inAir = (physics.numCollisions == 0);

        if (physics.inAir)
            Debug.Log("In Air");
    }
}