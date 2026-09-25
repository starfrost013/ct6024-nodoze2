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

        // this garage is used up, so don't bother a;;pwomg the, tp dp sp 
        if (used)
            return; 

        // Put *ALL* colliders on CarBody!
        Car car = other.gameObject.transform.parent.gameObject.GetComponent<Car>();

        if (car != null)
        {
            CarModifier modifier = car.GetCarModifiers();

            float fuelPercentHealed = modifier.refuelGaragePercent / 100.0f;
           
            if (car.physics.fuelCurrent > modifier.fuelMax)
                car.physics.fuelCurrent = modifier.fuelMax;
            
            // le sigh
            GameModeRaceMode raceMode = GameManager.mode as GameModeRaceMode;

            raceMode.raceConfigData.timeLimit += raceMode.raceConfigData.checkpointTimeGain;

            used = true; 
        }
    }

    /// <summary>
    /// fired on enter so the player can't refuel multiple times
    /// </summary>
    private bool used; 
}
