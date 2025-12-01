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
        internal float deceleration;
        internal float airDeceleration;                             // deceleration in the air

        internal float steeringIntensity;                           // the intensity of the steering

        internal CarSteeringType steeringType;

        internal float boostAmount;
        internal float boostAccelerationForward;
        internal float boostAccelerationSteering;

        // maybe needs to become an enum
        internal bool boosting;
        internal bool boostEnding;                        // lets us slowly ramp down


        internal Vector3 velocity;
    };


    internal enum CarSteeringType
    {
        Automatic = 0,
        Manual = 1,
    }

    //
    // FIELDS
    //

    TextAsset config;
  
    // This is transient like it is in real life
    float coolness;

    //todo: move to "BaseObject" class

    // generic epsilon
    const float EPSILON_MIN = 0.003f;

    PhysicsInfo physics;

    GameObject parent;

    //
    // METHODS
    //

    protected new void Start()
    {
        base.Start();

        // disregard non-custom fores

        //TODO: WHEEL PHYSICS

        // get the car
        parent = transform.gameObject;

        // find the wheels

        GameObject wheel_left_back = parent.transform.Find("wheel_left_back").gameObject;
        GameObject wheel_left_front = parent.transform.Find("wheel_left_front").gameObject;
        GameObject wheel_right_back = parent.transform.Find("wheel_right_back").gameObject;
        GameObject wheel_right_front = parent.transform.Find("wheel_right_front").gameObject;

        Debug.Assert(wheel_left_back && wheel_left_front && wheel_right_back && wheel_right_front, "Can't find car wheels!");

        // it is possible to use components and create many of these basecars with different gaemassetconfigfile components that have editor-modifiable resource paths
        // BUT IS IT GOOD? Who can know??? We'll have to try it!!
        config = AssetManager.LoadAsset<TextAsset>("Cars/CarTest"); // in the future cars will overload from this

        ConfigParser.ParseCfg(config.text);

        physics.topSpeed = float.Parse(ConfigParser.GetValue("Handling", "TopSpeed"));
        physics.topSpeedBoost = float.Parse(ConfigParser.GetValue("Handling", "TopSpeedBoost"));
        physics.accelerationForward = float.Parse(ConfigParser.GetValue("Handling", "AccelerationForward"));
        physics.accelerationSteering = float.Parse(ConfigParser.GetValue("Handling", "AccelerationSteering"));

        physics.deceleration = float.Parse(ConfigParser.GetValue("Handling", "Deceleration"));

        physics.boostAmount = float.Parse(ConfigParser.GetValue("Handling", "BoostAmount"));
        physics.boostAccelerationForward = float.Parse(ConfigParser.GetValue("Handling", "BoostAccelerationForward"));
        physics.boostAccelerationSteering = float.Parse(ConfigParser.GetValue("Handling", "BoostAccelerationSteering"));

        physics.steeringIntensity = float.Parse(ConfigParser.GetValue("Handling", "SteeringIntensity"));
        physics.steeringType = (CarSteeringType)Enum.Parse(typeof(CarSteeringType), ConfigParser.GetValue("Handling", "SteeringType"));

        // fix screwed up model shit
    }

    // FixedUpdate contains our controls so they feel decent regardless of fraemrate

    private void FixedUpdate()
    {
        // WHEN THIS BECOMES A RIGIDBODY, USE FixedUpdate
    }

    private void Update()
    {
        // I don't have time to use ISp
        Vector3 torque = new();

        bool moveInput = false;
        bool steeringInput = false;

        // check if we stopped boosting
        bool boostingTemp = physics.boosting;
        physics.boosting = Input.GetKey(KeyCode.LeftShift);

        if (boostingTemp && !physics.boosting)
            physics.boostEnding = true;

        // boost isn't finished until we slow down after boost is done
        if (physics.boostEnding
            && (Mathf.Abs(physics.velocity.x) < physics.topSpeed)
            && (Mathf.Abs(physics.velocity.y) < physics.topSpeed
            && (Mathf.Abs(physics.velocity.z) < physics.topSpeed)))
        {
            physics.boostEnding = false;
        }

        Vector3 forwardAccelerationForThisFrame = transform.forward * physics.accelerationForward * Time.deltaTime;
        Vector3 steeringAccelerationForThisFrame = transform.right * physics.accelerationSteering * Time.deltaTime;

        // if we ARE boosting, apply boost accel.
        // if we RECENTLY STOPPED boosting, apply zero accel.
        // otherwise, apply 
        if (physics.boosting)
        {
            forwardAccelerationForThisFrame = transform.forward * physics.boostAccelerationForward * Time.deltaTime;
            steeringAccelerationForThisFrame = transform.right * physics.boostAccelerationSteering * Time.deltaTime;
        }
        else if (physics.boostEnding)
            forwardAccelerationForThisFrame = steeringAccelerationForThisFrame = new Vector3(0.0f, 0.0f, 0.0f); // only apply natural deceleration

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
        && (physics.velocity.magnitude > EPSILON_MIN))
        {
            steeringInput = true;
            torque.y = -physics.steeringIntensity; // normalised?
            physics.velocity += steeringAccelerationForThisFrame;
        }

        if (Input.GetKey(KeyCode.RightArrow)
        || Input.GetKey(KeyCode.D)
        && (physics.velocity.magnitude > EPSILON_MIN))
        {
            steeringInput = true;
            torque.y = physics.steeringIntensity; // normalised?
            physics.velocity += -steeringAccelerationForThisFrame;
        }

        //
        // APPLICATION
        //

        // apply some natural decay
        if ((!moveInput && !steeringInput)
            || physics.boostEnding) // should we lock this?
        {
            physics.velocity.Scale(new Vector3(physics.deceleration, physics.deceleration, physics.deceleration));
        }

        Debug.Log("Velocity: " + physics.velocity.x + " " + physics.velocity.y + " " + physics.velocity.z);

        float topSpeed = physics.topSpeed;

        // test
        // if the player is boosting we don't want them 
        if (physics.boosting || physics.boostEnding)
            topSpeed = physics.topSpeedBoost;

        // anti-big rigs (apply this one at a time)
        if (physics.velocity.x > topSpeed)
            physics.velocity.Set(topSpeed, physics.velocity.y, physics.velocity.z);
        else if (physics.velocity.x < -topSpeed)
            physics.velocity.Set(-topSpeed, physics.velocity.y, physics.velocity.z);

        if (physics.velocity.y > topSpeed)
            physics.velocity.Set(physics.velocity.x, topSpeed, physics.velocity.z);
        else if (physics.velocity.y < -topSpeed)
            physics.velocity.Set(physics.velocity.x, -topSpeed, physics.velocity.z);

        if (physics.velocity.z > topSpeed)
            physics.velocity.Set(physics.velocity.x, physics.velocity.y, topSpeed);
        else if (physics.velocity.z < -topSpeed)
            physics.velocity.Set(physics.velocity.x, physics.velocity.y, -topSpeed);

        //thisRigidbody.AddForce(physics.velocity.x, physics.velocity.y, physics.velocity.z, ForceMode.VelocityChange);
        //thisRigidbody.AddTorque(torque.x, torque.y, torque.z);

        transform.SetPositionAndRotation(new(transform.position.x + physics.velocity.x,
            transform.position.y + physics.velocity.y,
            transform.position.z + physics.velocity.z),
        
            Quaternion.Euler(transform.rotation.eulerAngles.x + torque.x,
            transform.rotation.eulerAngles.y + torque.y,
            transform.rotation.eulerAngles.z + torque.z));

        // apply motion

        Camera.main.transform.position = transform.position + (transform.forward * 3.0f);
        Camera.main.transform.position += new Vector3(0.0f, 1.4f, 0.0f);

        // this is going to need a lot of work

        // Fix when model correctly imported
        Vector3 carRot = transform.rotation.eulerAngles;
        Camera.main.transform.rotation = Quaternion.Euler(carRot.x, carRot.y + 180, carRot.z);
    }
}