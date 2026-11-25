using UnityEngine;

// CarDead.cs
// A "dead" car, used to spawn a noncontrollable c
// Inherits from other car classes and loads config in the same way as other cars

class CarDead : Car
{    
    protected override void Start()
    {
        base.Start();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    /// <summary>
    /// get an undead clone of a car
    /// </summary>
    /// <param name="car">the car to get an undead clone of</param>
    /// <returns>dead car</returns>
    public static CarDead GetUndeadCloneOfCar(Car car)
    {
        GameObject objv = new("DeadCar");
        CarDead deadCar = objv.AddComponent<CarDead>();

        deadCar.configText = car.configText;
        deadCar.configFilePath = car.configFilePath;

        deadCar.LoadConfigFromString(car.configText.text);
        return deadCar;
    }

}
