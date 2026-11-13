using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// UI Manager
/// 
/// It manages the UI. Who could have thought?
/// 
/// Each menu is a prefab.
/// SO you specify the menu prefabs in ehre and it loads it.
/// 
/// If we have time, convert e.g. the PostRace menu to use this.
/// </summary>
static class UIManager
{
    private const string UI_PATH = "UI/";

    internal static GameObject[] menus;

    private static List<GameObject> _currentMenus = new(); // compile time initialisation
    internal static List<GameObject> currentMenus
    {
        get { return _currentMenus; }
        private set { value = currentMenus; }
    }

    internal static void Init()
    {
        menus = AssetManager.LoadAssetsInFolder<GameObject>(UI_PATH);

        if (menus == null)
        {
            Debug.LogError("Failed to load menus!");
            return;
        }
        
    }

    internal static GameObject GetUIByName(string name)
    {
        foreach (GameObject menu in menus)
        {
            if (menu.name == name)
                return menu;
        }

        return null;
    }

    internal static void AddUI(string name)
    {
        GameObject menu = GetUIByName(name);

        if (menu == null)
        {
            Debug.LogError("UIManager::AddUI - Tried to get INVALID menu " + name);
            return;
        }

        // instantiate and add 
        GameObject clone = GameObject.Instantiate(menu); 
        
        currentMenus.Add(clone); 
    }

    internal static void DestroyUI(string name)
    {
        GameObject ui = GetUIByName(name);

        if (ui != null)
        {
            GameObject.Destroy(ui);
            currentMenus.Remove(ui);
        }
        else
            Debug.LogError("UIManager::DestroyUI - UI was not in UI list");

    }
    
    internal static void EnableUI(string name)
    {
        GameObject ui = GetUIByName(name);

        if (ui != null)
            ui.SetActive(true);
        else
            Debug.LogError("UIManager::EnableUI - UI was not in UI list");        
    }

    internal static void DisableUI(string name)
    {
        GameObject ui = GetUIByName(name);

        if (ui != null)
            ui.SetActive(false);
        else
            Debug.LogError("UIManager::EnableUI - UI was not in UI list");         
    }

}
