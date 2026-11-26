using Unity.VisualScripting;
using UnityEngine;

// CarDead.cs
// A "dead" car, used to spawn a noncontrollable c
// Inherits from other car classes and loads config in the same way as other cars

class CarDead : Car
{    
    protected override void Start()
    {

    }

    protected override void FixedUpdate()
    {
        //base.FixedUpdate();
    }

    /// <summary>
    /// get an undead clone of a car
    /// </summary>
    /// <param name="car">the car to get an undead clone of</param>
    /// <returns>dead car</returns>
    public static CarDead GetUndeadCloneOfCar(GameObject obj)
    {
        GameObject objNew = GameObject.Instantiate(obj);

        Car car = objNew.GetComponent<Car>();

        if (car == null)
        {
            Debug.LogError("Failed to kill car for GOAP: no Car component! (Game object name is " + obj.name + ")");
            return null;
        }

        CarDead deadCar = objNew.AddComponent<CarDead>();

        deadCar.configText = car.configText;
        deadCar.configFilePath = car.configFilePath;
        deadCar.accelerationCurve = car.accelerationCurve;
        deadCar.accelerationCurveNumTicks = car.accelerationCurveNumTicks;
        deadCar.brakeNormalCurve = car.brakeNormalCurve;
        deadCar.brakeNormalCurveNumTicks = car.brakeNormalCurveNumTicks;
        deadCar.jumpCurve = car.jumpCurve;
        deadCar.jumpCurveNumTicks = car.jumpCurveNumTicks;
        deadCar.steeringCurve = car.steeringCurve;
        deadCar.steeringCurveNumTicks = car.steeringCurveNumTicks;

        // TEMP test code
        // move it to a point randomly within a cone for now
        Vector3 positionDisplacement = Random.onUnitSphere * 6;
        // ensure its not too close to the player
        positionDisplacement.x += 1;
        positionDisplacement.y = Mathf.Clamp(Mathf.Abs(positionDisplacement.y), 0.1f, 1.0f);
        positionDisplacement.z += 1;

        deadCar.transform.position = car.transform.position + positionDisplacement;

        // TODO: select all cars to load frm. but jsut clone the player for now. Select from the car manager.
        deadCar.LoadConfigFromString(GameManager.player.car.configText.text);

        // update the internal stuff
        deadCar.physRigidbody = deadCar.GetComponent<Rigidbody>(); // remove this if we decide to remove basephysicsobject


        deadCar.physRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // find the wheels

        // check the wheels actually have colliders
        deadCar.wheelLeftBack = car.wheelLeftBack;
        deadCar.wheelLeftFront = car.wheelLeftFront;
        deadCar.wheelRightBack = car.wheelRightBack;
        deadCar.wheelRightFront = car.wheelRightFront;
        
        deadCar.physics.wheelLeftBackCollider = car.wheelLeftBack.GetComponent<WheelCollider>();
        deadCar.physics.wheelLeftFrontCollider = car.wheelLeftFront.GetComponent<WheelCollider>();
        deadCar.physics.wheelRightBackCollider = car.wheelRightBack.GetComponent<WheelCollider>();
        deadCar.physics.wheelRightFrontCollider = car.wheelRightFront.GetComponent<WheelCollider>();

        GameObject.Destroy(car);
        return deadCar;
    }

}
