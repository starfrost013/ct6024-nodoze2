using UnityEngine;

/// <summary>
/// Item
/// 
/// Base item class.
/// 
/// Each item class has a manager that handles how it gets loaded and a class that actually defines the class.
/// This is intended to give maximum flexibility while still allowing shared behaviour like the inventory.
/// 
/// The item container is implemented as the inventory
/// </summary>
public class Item : MonoBehaviour
{
    /// <summary>
    /// don't allow this item to be duplicated in the inventroy
    /// </summary>
    internal bool preventDuplication;

    /// <summary>
    /// this item is currently selectred
    /// </summary>
    internal bool isActive = false;

    /// <summary>
    /// the name of the item
    /// </summary>
    public string itemName;
}
