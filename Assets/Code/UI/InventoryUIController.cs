//
// ND2 (Nodoze-2) UoG
// Version 1.0 (CT5010): © 2025-2026 NotDominoes Team 
// Version 2.0 (CT6024): © 2025-2027 Connor Hyde (starfrost)
// 
// InventoryUIController
// The inventory ui is controlled here. Each individual tile has this script attached; this is ENTIRELY VISUAL AND HAS NO RELATION TO THE ACTUAL INVENTORY SYSTEMN

using UnityEngine; 

class InventoryUIController : MonoBehaviour
{
    [SerializeField]
    private int inventoryIndex = 0; 
    
    public void OnClick()
    {
        GameManager.player.stats.inventory.SetActivatedIndex(inventoryIndex, GameManager.player.stats.inventory.IsIndexActivated(inventoryIndex));
    }
}