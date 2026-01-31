using System;
using System.Net.NetworkInformation;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.STP;

// It's a car :D
internal class Car : BasePhysicsObject
{
    //
    // STRUCTS
    // 

    internal struct PhysicsInfo
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
        internal float maxRotationTorque;                           // maximum torque

        internal float steeringIntensity;                           // the intensity of the steering

        internal CarSteeringType steeringType;

        // Boosting characteristics of the car
        internal float boostAmount;
        internal float boostAccelerationForward;
        internal float boostAccelerationSteering;
        internal float boostAccelerationForwardAir;
        internal float boostAccelerationSteeringAir;

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

    private TextAsset configText;

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

    // parent object (to prevent endless "parent = transform.gameObject")
    GameObject parent;

    //
    // METHODS
    //

    /* Loads the configuration */
    internal void LoadConfig()
    {
        configText = AssetManager.LoadAsset<TextAsset>(configFilePath);
        ConfigParser.Parse(configText.text);

        try
        {
            physics.topSpeed = float.Parse(ConfigParser.GetValue("Handling", "TopSpeed"));
            physics.topSpeedBoost = float.Parse(ConfigParser.GetValue("Handling", "TopSpeedBoost"));
            physics.accelerationForward = float.Parse(ConfigParser.GetValue("Handling", "AccelerationForward"));
            physics.accelerationSteering = float.Parse(ConfigParser.GetValue("Handling", "AccelerationSteering"));
            physics.accelerationForwardAir = float.Parse(ConfigParser.GetValue("Handling", "AccelerationForwardAir"));
            physics.accelerationSteeringAir = float.Parse(ConfigParser.GetValue("Handling", "AccelerationSteeringAir"));

            physics.deceleration = float.Parse(ConfigParser.GetValue("Handling", "Deceleration"));
            physics.decelerationSteering = float.Parse(ConfigParser.GetValue("Handling", "DecelerationSteering"));
            physics.decelerationAir = float.Parse(ConfigParser.GetValue("Handling", "DecelerationAir"));
            physics.decelerationChangeDirection = float.Parse(ConfigParser.GetValue("Handling", "DecelerationChangeDirection"));
            physics.decelerationChangeDirectionSteering = float.Parse(ConfigParser.GetValue("Handling", "DecelerationChangeDirectionSteering"));
            physics.maxRotationTorque = float.Parse(ConfigParser.GetValue("Handling", "MaxTorque"));

            physics.boostAmount = float.Parse(ConfigParser.GetValue("Handling", "BoostAmount"));
            physics.boostAccelerationForward = float.Parse(ConfigParser.GetValue("Handling", "BoostAccelerationForward"));
            physics.boostAccelerationSteering = float.Parse(ConfigParser.GetValue("Handling", "BoostAccelerationSteering"));
            physics.boostAccelerationForwardAir = float.Parse(ConfigParser.GetValue("Handling", "BoostAccelerationForwardAir"));
            physics.boostAccelerationSteeringAir = float.Parse(ConfigParser.GetValue("Handling", "BoostAccelerationSteeringAir"));

            physics.steeringIntensity = float.Parse(ConfigParser.GetValue("Handling", "SteeringIntensity"));
            physics.steeringType = (CarSteeringType)Enum.Parse(typeof(CarSteeringType), ConfigParser.GetValue("Handling", "SteeringType"));

        }
        catch (Exception e)
        {
            Debug.LogError("FAILED to load car settings!!!: " + e);
        }

        Debug.Assert(configText, "You didn't load a configuration for this car!!!");
       
    }

    protected new void Start()
    {
        // prevent crash
        if (!configText)
            return;

        base.Start();
        thisRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // get the car
        parent = transform.gameObject;

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

    private void FixedUpdate()
    {

        // I don't have time to use ISP sorry!

        bool moveInput = false;
        bool steeringInput = false;

        // check if we stopped boosting
        physics.boosting = Input.GetKey(KeyCode.LeftShift);
        physics.boostEnding = Input.GetKeyUp(KeyCode.LeftShift);

        // boost isn't finished until we slow down after boost is done
        if (physics.boostEnding
            && (Mathf.Abs(physics.forwardTorque) < physics.topSpeed))
        {
            physics.boostEnding = false;
        }

        float forwardAccelHandlingForThisFrame = (physics.inAir) ? physics.accelerationForwardAir : physics.accelerationForward;
        float steeringAccelHandlingForThisFrame = (physics.inAir) ? physics.accelerationSteeringAir : physics.accelerationSteering;

        // if we ARE boosting, apply boost accel.
        // if we RECENTLY STOPPED boosting, apply zero accel.
        // otherwise, apply 
        if (physics.boosting)
        {
            forwardAccelHandlingForThisFrame = (physics.inAir) ? physics.boostAccelerationForwardAir : physics.boostAccelerationForward;
            steeringAccelHandlingForThisFrame = (physics.inAir) ? physics.boostAccelerationSteeringAir : physics.boostAccelerationSteering;
        }
        else if (physics.boostEnding)
            forwardAccelHandlingForThisFrame = steeringAccelHandlingForThisFrame = 0.0f;  // only apply natural deceleration of boost is ending

        // New code does this calculation automatically - Jan 28, 2025 

        float forwardAccelerationForThisFrame = forwardAccelHandlingForThisFrame * Time.fixedDeltaTime;
        float steeringAccelerationForThisFrame = steeringAccelHandlingForThisFrame * Time.fixedDeltaTime;

        // if the boost is ending - we want to decelerate
        // we also want to rapidly change direction if we are steering

        if (Input.GetKey(KeyCode.UpArrow)
            || Input.GetKey(KeyCode.W))
        {
            moveInput = true;

            if (physics.forwardTorque > 0)
                physics.forwardTorque -= physics.decelerationChangeDirection;

            physics.forwardTorque += -forwardAccelerationForThisFrame;
        }

        if (Input.GetKey(KeyCode.DownArrow)
            || Input.GetKey(KeyCode.S))
        {
            moveInput = true;

            if (physics.forwardTorque < 0)
                physics.forwardTorque += physics.decelerationChangeDirection;

            physics.forwardTorque += forwardAccelerationForThisFrame;
        }

        // multiply so the car can have a smaller turning circle as it gets faster
        float steeringChangeFactor = physics.steeringIntensity;

        if (Math.Abs(physics.forwardTorque) > 1.0f)
            steeringChangeFactor *= Math.Abs(physics.forwardTorque) / 4.0f;

        if (Input.GetKey(KeyCode.LeftArrow)
            || Input.GetKey(KeyCode.A)
        && (Math.Abs(physics.forwardTorque) > EPSILON_MIN))
        {
            steeringInput = true;

            //physics.forwardTorque -= steeringAccelerationForThisFrame;

            if (physics.rotationTorque > 0)
                physics.rotationTorque -= physics.decelerationChangeDirectionSteering;
            else if (Math.Abs(physics.rotationTorque) < physics.maxRotationTorque)
                physics.rotationTorque -= steeringChangeFactor; // normalised?
        }

        if (Input.GetKey(KeyCode.RightArrow)
            || Input.GetKey(KeyCode.D)
            && (Math.Abs(physics.forwardTorque) > EPSILON_MIN))
        {
            steeringInput = true;

            //physics.forwardTorque += steeringAccelerationForThisFrame;

            if (physics.rotationTorque < 0)
                physics.rotationTorque += physics.decelerationChangeDirectionSteering;
            else if (Math.Abs(physics.rotationTorque) < physics.maxRotationTorque)
                physics.rotationTorque += steeringChangeFactor; // normalised?
        }

        //
        // APPLICATION OF THE MOMENTUM OF THE CAR
        //

        // this code is awful 

        // apply some natural decay
        if ((!moveInput && !steeringInput)
            || physics.boostEnding) // should we lock this?
        {
            if (physics.inAir)
                physics.forwardTorque *= physics.decelerationAir;
            else
                physics.forwardTorque *= physics.deceleration;
        }

        if (!steeringInput)
            physics.rotationTorque *= physics.decelerationSteering;

        // Debug.Log("Velocity: " + physics.velocity.x + " " + physics.velocity.y + " " + physics.velocity.z);

        float topSpeed = physics.topSpeed;

        // test
        // if the player is boosting we don't want them 
        if (physics.boosting || physics.boostEnding)
            topSpeed = physics.topSpeedBoost;

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

        Camera.main.transform.position = transform.position + (transform.forward * 5.0f);
        Camera.main.transform.position += new Vector3(0.0f, 1.4f, 0.0f);

        // this is going to need a lot of work
        
        // Fix when model correctly imported
        Vector3 carRot = transform.rotation.eulerAngles;
        Camera.main.transform.rotation = Quaternion.Euler(carRot.x, carRot.y + 180, carRot.z);
        
    }

    private void Update()
    {
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
        {
            Debug.Log("In Air");

        }
    }
}