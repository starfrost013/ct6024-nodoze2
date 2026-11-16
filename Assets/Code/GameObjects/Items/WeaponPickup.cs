using UnityEngine;

/// <summary>
/// A weapon pickup.
/// 
/// Only picks up weapon items because I messed up designing the item system...I might fix it laterr
/// </summary>
class WeaponPickup : MonoBehaviour
{
    /// <summary>
    /// filename
    /// </summary>
    [SerializeField, Tooltip("Resources/Weapons file to use")]
    string itemName; 

    private void OnTriggerEnter()
    {
        Weapon weapon = WeaponManager.GetWeaponByName(itemName);

        if (weapon == null)
        {
            Debug.LogWarning("WeaponPickup::OnTriggerEnter - weapon " + itemName + " doesn't exist...Add it to the Weapons folder...");
        }
        else
        {
            GameManager.player.stats.inventory.AddItem(weapon);
        }
    }
}