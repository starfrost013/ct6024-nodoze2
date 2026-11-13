using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Inventory
/// 
/// The inventory...that the player has
/// </summary>
class Inventory
{
    private List<Item> items = new();

    public void AddItem(Item item)
    {
        if (item == null)
        {
            Debug.LogError("Inventory::AddItem - item is NULL!");
            return; 
        }

        items.Add(item);    
    }

    public void RemoveItem(Item item)
    {
        if (!items.Remove(item))
            Debug.LogError("Inventory::RemoveItem - item was not in the list lol");
    }


}