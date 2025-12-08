using UnityEngine;

// This is the finish line.
// It lets us knw
class FinishLine : MonoBehaviour
{
    GameModeRaceMode mode;

    //maybe put this in a RaceINfo class
    Car car;

    void Start()
    {
        car = FindFirstObjectByType<Car>(); 
    }

    private void OnTriggerEnter(Collider other)
    {
        // don't do anything if it isn't racemode
        if (GameManager.GetGameState() != GameManager.GameState.RaceMode)
            return; 

        if (other.gameObject.GetType() == typeof(Car))
        {
            GameManager.SetGameState(GameManager.GameState.RaceFinished);
        }
    }

}
