using UnityEngine;

// This is the finish line.
// It lets us know when we are done
class FinishLine : MonoBehaviour
{
    /// <summary>
    /// the exit type
    /// </summary>
    enum FinishLineExitType
    {
        Normal = 0,
        Special = 1,
    }; 

    [SerializeField]
    FinishLineExitType exitType = FinishLineExitType.Normal; 
    
    private void OnTriggerEnter(Collider other)
    {
        // don't do anything if it isn't racemode
        if (GameManager.GetGameState() != GameManager.GameModeEnum.RaceMode)
            return; 
    
        // the collision is the CarBody`
        if (other.gameObject.transform.parent.gameObject.GetComponent<Car>())
        {

            // tell racemode we are finished
            GameModeRaceMode mode = (GameModeRaceMode)GameManager.mode;

            if (mode.raceState == GameModeRaceMode.RaceState.Active)
            {
                switch (exitType)
                {
                    case FinishLineExitType.Normal:
                        mode.raceState = GameModeRaceMode.RaceState.FinishedNormal;
                        Debug.Log("Finished race normally!");
                        break;
                    case FinishLineExitType.Special:
                        mode.raceState = GameModeRaceMode.RaceState.FinishedSpecial;
                        Debug.Log("Finished race with special exit!");
                        break;
                }
            }
        }
    }

}
