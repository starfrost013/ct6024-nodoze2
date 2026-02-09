using UnityEngine;

// This is the finish line.
// It lets us know when we are done
class FinishLine : MonoBehaviour
{

    void Start()
    {
    
    }

    private void OnTriggerEnter(Collider other)
    {
        // don't do anything if it isn't racemode
        if (GameManager.GetGameState() != GameManager.GameModeEnum.RaceMode)
            return; 

        if (other.gameObject.GetComponent<Car>())
        {
            Debug.Log("You got to the end of the race!");
            GameManager.SetGameState(GameManager.GameModeEnum.RaceFinished);
        }
    }

}
