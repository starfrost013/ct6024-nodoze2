using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// WeaponManager
/// 
/// Loads weapons for ND2
/// </summary>
static class WeaponManager
{
    private const string WEAPON_PATH = "Weapons/";

    internal static GameObject[] weapons;

    internal static void Init()
    {
        weapons = AssetManager.LoadAssetsInFolder<GameObject>(WEAPON_PATH);

        if (weapons == null)
        {
            Debug.LogError("Failed to load weapons!");
            return;
        }
    }
}
