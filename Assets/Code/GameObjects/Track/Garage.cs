using UnityEngine;

// This is the finish line.
// It lets us knw
class Garage : MonoBehaviour
{
    void Start()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        // don't do anything if it isn't racemode
        if (GameManager.GetGameState() != GameManager.GameModeEnum.RaceMode)
            return;

        Car car = other.gameObject.GetComponent<Car>();

        if (car != null)
        {
            Car.PhysicsInfo physInfo = car.GetPhysicsInfo();
            CarModifier modifier = car.GetCarModifiers();

            physInfo.fuelCurrent += (modifier.fuelMax * (modifier.refuelGaragePercent * 100.0f));

            car.SetPhysicsInfo(physInfo);
        }
    }

}
