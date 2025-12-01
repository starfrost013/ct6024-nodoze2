using System;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.STP;

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

    TextAsset config;

    // Characteristics of the car
    float topSpeed;
    float topSpeedBoost;
    float accelerationForward;
    float accelerationSteering;
    float deceleration;

    float steeringIntensity;                            // the intensity of the steering

    CarSteeringType steeringType;

    float boostAmount;
    float boostAcceleration;

    // This is transient like it is in real life
    float coolness;

    //todo: move to "BaseObject" class
    Vector3 velocity;

    const float EPSILON_MIN = 0.01f;

    GameObject parent;

    //
    // METHODS
    //

    private void Start()
    {
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

        topSpeed = float.Parse(ConfigParser.GetValue("Handling", "TopSpeed"));
        topSpeedBoost = float.Parse(ConfigParser.GetValue("Handling", "TopSpeedBoost"));
        accelerationForward = float.Parse(ConfigParser.GetValue("Handling", "AccelerationForward"));
        accelerationSteering = float.Parse(ConfigParser.GetValue("Handling", "AccelerationSteering"));

        deceleration = float.Parse(ConfigParser.GetValue("Handling", "Deceleration"));

        boostAmount = float.Parse(ConfigParser.GetValue("Handling", "BoostAmount"));
        boostAcceleration = float.Parse(ConfigParser.GetValue("Handling", "BoostAcceleration"));
        steeringIntensity = float.Parse(ConfigParser.GetValue("Handling", "SteeringIntensity"));
        steeringType = (CarSteeringType)Enum.Parse(typeof(CarSteeringType), ConfigParser.GetValue("Handling", "SteeringType"));

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

        Vector3 rotation = transform.rotation.eulerAngles;
        bool moveInput = false;
        bool steeringInput = false;

        if (Input.GetKey(KeyCode.UpArrow)
            || Input.GetKey(KeyCode.W))
        {
            moveInput = true;
            velocity += -transform.forward * accelerationForward * Time.deltaTime;
        
        }

        if (Input.GetKey(KeyCode.DownArrow)
            || Input.GetKey(KeyCode.S))
        {
            moveInput = true;
            velocity += transform.forward * accelerationForward * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.LeftArrow)
        || Input.GetKey(KeyCode.A)
        && (velocity.magnitude > EPSILON_MIN))
        {
            steeringInput = true;
            rotation.y = transform.rotation.eulerAngles.y - steeringIntensity; // normalised?
            velocity += -transform.right * accelerationSteering * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.RightArrow)
        || Input.GetKey(KeyCode.D)
        && (velocity.magnitude > EPSILON_MIN))
        {
            steeringInput = true;
            rotation.y = transform.rotation.eulerAngles.y + steeringIntensity; // normalised?
            velocity += transform.right * accelerationSteering * Time.deltaTime;
        }

        // apply some natural decay
        if (!moveInput && !steeringInput)
        {
            velocity.Scale(new Vector3(deceleration, deceleration, deceleration));
        }

        Debug.Log("Velocity: " + velocity.x + " " + velocity.y + " " + velocity.z);

        // anti-big rigs (apply this one at a time)
        if (velocity.x > topSpeed)
            velocity.Set(topSpeed, velocity.y, velocity.z);
        else if (velocity.x < -topSpeed)
            velocity.Set(-topSpeed, velocity.y, velocity.z);

        if (velocity.y > topSpeed)
            velocity.Set(velocity.x, topSpeed, velocity.z);
        else if (velocity.y < -topSpeed)
            velocity.Set(velocity.x, -topSpeed, velocity.z);

        if (velocity.z > topSpeed)
            velocity.Set(velocity.x, velocity.y, topSpeed);
        else if (velocity.z < -topSpeed)
            velocity.Set(velocity.x, velocity.y, -topSpeed);


        transform.SetPositionAndRotation(new(transform.position.x + velocity.x,
            transform.position.y + velocity.y,
            transform.position.z + velocity.z),

            Quaternion.Euler(rotation.x, rotation.y, rotation.z));

        // apply motion

        Camera.main.transform.position = transform.position + (transform.forward * 3.0f);
        Camera.main.transform.position += new Vector3(0.0f, 1.4f, 0.0f);

        // this is going to need a lot of work

        // Fix when model correctly imported
        Vector3 carRot = transform.rotation.eulerAngles;
        Camera.main.transform.rotation = Quaternion.Euler(carRot.x, carRot.y + 180, carRot.z);
    }
}