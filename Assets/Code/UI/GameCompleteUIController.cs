
using UnityEngine;

internal class GameCompleteUIController : MonoBehaviour
{ 
    public void OnMainMenuClicked()
    {
        GameManager.SetGameState(GameManager.GameModeEnum.MainMenu);
    }
}
