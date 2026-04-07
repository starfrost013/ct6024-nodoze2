using UnityEngine;

// This is the finish line.
// It lets us know when we are done
class FinishLine : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // don't do anything if it isn't racemode
        if (GameManager.GetGameState() != GameManager.GameModeEnum.RaceMode)
            return; 

        if (other.gameObject.GetComponent<Car>())
        {
            Debug.Log("You got to the end of the race!");

            // tell racemode we are finished
            GameModeRaceMode mode = (GameModeRaceMode)GameManager.mode;

            mode.raceState = GameModeRaceMode.RaceState.Finished;

            GameManager.SetGameState(GameManager.GameModeEnum.RaceFinished);
        }
    }

}
