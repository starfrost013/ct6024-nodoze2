using UnityEngine;

// This is the finish line.
// It lets us knw
class FinishLine : MonoBehaviour
{
    GameModeRaceMode mode; 

    private void Start()
    {
        if (GameManager.GameState != GameManager.GameState.RaceMode)
        {

        }
        // If it's not race mode, we have no reason to exist
        Destroy(gameObject);
    }
}
