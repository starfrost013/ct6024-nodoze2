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

    private Weapon weapon;

    void Start()
    {
        // Spawn an instance of the weapon prefab so we can pick up the weapon

        weapon = WeaponManager.GetWeaponByName(itemName);

        GameObject.Instantiate(weapon, transform.position, Quaternion.identity); // we don't care about this
    }

    void FixedUpdate()
    {
        // todo: make configurable?
        transform.localEulerAngles = transform.localEulerAngles + new Vector3(0, 5, 0); // spin around
    }

    private void OnTriggerEnter()
    {

        if (weapon == null)
            Debug.LogWarning("WeaponPickup::OnTriggerEnter - weapon " + itemName + " doesn't exist...Add it to the Weapons folder...");

        else
            GameManager.player.stats.inventory.AddItem(weapon);

    }
}