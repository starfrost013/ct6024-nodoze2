using Unity.VisualScripting;
using UnityEngine;

// CarDead.cs
// A "dead" car, used to spawn a noncontrollable c
// Inherits from other car classes and loads config in the same way as other cars

class CarDead : Car
{    
    protected override void Start()
    {
        //physRigidbody = this.AddComponent<Rigidbody>();

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

        // TEMP test code
        // move it to a point randomly within a cone for now
        Vector3 positionDisplacement = Random.onUnitSphere * 6;
        // ensure its not too close to the player
        positionDisplacement.x += 1;
        positionDisplacement.y = Mathf.Clamp(Mathf.Abs(positionDisplacement.y), 0.1f, 1.0f);
        positionDisplacement.z += 1;

        deadCar.transform.position = car.transform.position + positionDisplacement;

        //deadCar.LoadConfigFromString(car.configText.text);
        GameObject.Destroy(car);
        return deadCar;
    }

}
