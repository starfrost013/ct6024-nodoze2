using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Menu Manager
/// 
/// It manages the menus. Who could have thought?
/// 
/// Each menu is a prefab.
/// SO you specify the menu prefabs in ehre and it loads it.
/// 
/// If we have time, convert e.g. the PostRace menu to use this.
/// </summary>
static class UIManager
{
    private const string MENU_PATH = "UI/";

    internal static GameObject[] menus;

    internal static GameObject currentMenu { get; private set; }

    internal static void Init()
    {
        menus = AssetManager.LoadAssetsInFolder<GameObject>(MENU_PATH);

        if (menus == null)
        {
            Debug.LogError("Failed to load menus!");
            return;
        }
    }

    internal static GameObject GetMenuByName(string name)
    {
        foreach (GameObject menu in menus)
        {
            if (menu.name == name)
                return menu;
        }

        return null;
    }

    internal static void DestroyCurrentMenu()
    {
        if (currentMenu != null)
        {
            GameObject.Destroy(currentMenu);
            currentMenu = null;
        }
    }

    internal static void SetCurrentMenu(string name)
    {
        GameObject menu = GetMenuByName(name);

        if (menu == null)
        {
            Debug.LogError("Tried to get INVALID menu " + name);
            return;
        }

        // remove the current menu if it exists
        if (currentMenu)
            GameObject.Destroy(currentMenu);

        currentMenu = GameObject.Instantiate(menu);
    }
}
