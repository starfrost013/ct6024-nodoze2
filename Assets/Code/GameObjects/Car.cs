using System;
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
        internal float accelerationForward;
        internal float accelerationSteering;
        internal float accelerationForwardAir;
        internal float accelerationSteeringAir;
        internal float deceleration;
        internal float decelerationAir;                            // deceleration in the air
        internal float maxTorque;                                   // deceleration in the air

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
        internal float velocity;
        internal float torque;

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
            physics.decelerationAir = float.Parse(ConfigParser.GetValue("Handling", "DecelerationAir"));
            physics.maxTorque = float.Parse(ConfigParser.GetValue("Handling", "MaxTorque"));

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
            && (Mathf.Abs(physics.velocity) < physics.topSpeed))
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
        float steeringAccelerationForThisFrame  =steeringAccelHandlingForThisFrame * Time.fixedDeltaTime;

        // if the boost is ending - we want to decelerate

        if (Input.GetKey(KeyCode.UpArrow)
            || Input.GetKey(KeyCode.W))
        {
            moveInput = true;
            physics.velocity += -forwardAccelerationForThisFrame;
        }

        if (Input.GetKey(KeyCode.DownArrow)
            || Input.GetKey(KeyCode.S))
        {
            moveInput = true;
            physics.velocity += forwardAccelerationForThisFrame;
        }

        if (Input.GetKey(KeyCode.LeftArrow)
        || Input.GetKey(KeyCode.A)
        && (physics.velocity > EPSILON_MIN))
        {
            steeringInput = true;
            if (Math.Abs(physics.torque) < physics.maxTorque)
                physics.torque -= physics.steeringIntensity; // normalised?
            physics.velocity += steeringAccelerationForThisFrame;
        }

        if (Input.GetKey(KeyCode.RightArrow)
        || Input.GetKey(KeyCode.D)
        && (physics.velocity > EPSILON_MIN))
        {
            steeringInput = true;
            if (Math.Abs(physics.torque) < physics.maxTorque)
                physics.torque += physics.steeringIntensity; // normalised?
            physics.velocity -= steeringAccelerationForThisFrame;
        }

        //
        // APPLICATION
        //

        // apply some natural decay
        if ((!moveInput && !steeringInput)
            || physics.boostEnding) // should we lock this?
        {
            if (physics.inAir)
                physics.velocity /= physics.decelerationAir;
            else
                physics.velocity /= physics.deceleration;
        }

        // Debug.Log("Velocity: " + physics.velocity.x + " " + physics.velocity.y + " " + physics.velocity.z);

        float topSpeed = physics.topSpeed;

        // test
        // if the player is boosting we don't want them 
        if (physics.boosting || physics.boostEnding)
            topSpeed = physics.topSpeedBoost;

        // anti-big rigs (apply this one at a time)
        if (physics.velocity > topSpeed)
            physics.velocity = topSpeed;
        else if (physics.velocity < -topSpeed)
            physics.velocity = -topSpeed;

        bool needFrontWheelDrive = (driveType == (CarDriveType.FrontWheelDrive) || (driveType == (CarDriveType.FourWheelDrive)));
        bool needRearWheelDrive = (driveType == (CarDriveType.RearWheelDrive) || (driveType == (CarDriveType.FourWheelDrive)));

        if (needFrontWheelDrive)
        {
            physics.wheelLeftBackCollider.motorTorque += physics.velocity;
            physics.wheelRightBackCollider.motorTorque += physics.velocity;
            physics.wheelLeftBackCollider.rotationSpeed += physics.torque;
            physics.wheelRightBackCollider.rotationSpeed += physics.torque;

            // TODO: rotate the wheels
        }

        if (needRearWheelDrive)
        {
            physics.wheelLeftFrontCollider.motorTorque += physics.velocity;
            physics.wheelRightFrontCollider.motorTorque += physics.velocity;
            physics.wheelLeftFrontCollider.rotationSpeed += physics.torque;
            physics.wheelRightFrontCollider.rotationSpeed += physics.torque;
        }

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