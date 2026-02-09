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
            CarModifier modifier = car.GetCarModifiers();

            float fuelPercentHealed = modifier.refuelGaragePercent / 100.0f;
           
            car.physics.fuelCurrent += (modifier.fuelMax * (modifier.refuelGaragePercent / 100.0f));
        
            if (car.physics.fuelCurrent > modifier.fuelMax)
                car.physics.fuelCurrent = modifier.fuelMax; 
        }
    }

}
