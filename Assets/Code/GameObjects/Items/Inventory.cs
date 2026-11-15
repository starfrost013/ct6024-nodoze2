using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Inventory
/// 
/// The inventory...that the player has
/// </summary>
class Inventory
{
    public List<Item> items { get; private set; } = new();

    public void AddItem(Item item)
    {
        if (item == null)
        {
            Debug.LogError("Inventory::AddItem - item is NULL!");
            return; 
        }

        items.Add(item);    
    }

    /// <summary>
    /// Returns a boolean indicating if inventory item id is activated.
    /// </summary>
    /// <param name="id">The ID of the inventory item to check</param>
    /// <returns>TRUE if the index is activated, FALSE if it is not activated or non-existent</returns>
    public bool IsIndexActivated(int id)
    {
        if (id < 0 
        || id >= items.Count)
        {
            Debug.LogWarning("Inventory::IsIndexActivated - Item ID " + id + "does not exist!");   
            return false;
        }

        return items[id].isActive;     
    }

    /// <summary>
    /// Checks if an inventory item exists with the id <paramref name="id"/>, and if it is, sets its active state to <paramref name="active"/>
    /// </summary>
    /// <param name="id">The ID of the inventory item to change the active state of</param>
    /// <param name="active">A boolean indicating the intended activation state of the item</param>
    /// <returns>Nothing - logs a warning message if the item ID does not exist/returns>
    public void SetActivatedIndex(int id, bool active)
    {
        if (id < 0 
        || id >= items.Count)
        {
            Debug.LogWarning("Inventory::SetActivatedIndex - Item ID " + id + "does not exist");   
            return;
        }

        items[id].isActive = active; 
    }

    public void RemoveItem(Item item)
    {
        if (!items.Remove(item))
            Debug.LogError("Inventory::RemoveItem - item was not in the list!");
    }

}