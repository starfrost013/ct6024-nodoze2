//
// ND2 (Nodoze-2) UoG
// Version 1.0 (CT5010): © 2025-2026 NotDominoes Team 
// Version 2.0 (CT6024): © 2025-2027 Connor Hyde (starfrost)
// 
// MainMenuUIController : the main menu ui controller
//

using UnityEngine;

class MainMenuUIController : MonoBehaviour
{
    public void MainClicked()
    {
        Debug.Log("Entering car select menu...");
        GameManager.SetGameState(GameManager.GameModeEnum.CarSelectMenu);
    }
    
    public void BossRushClicked()
    {
        
    }

    public void ExitClicked()
    {
        
    }
}