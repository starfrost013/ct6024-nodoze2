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
    public const string WEAPON_PATH = "Weapons/";

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

    internal static Weapon GetWeaponByName(string name)
    {
        foreach (GameObject weaponObject in weapons)
        {
            Weapon weapon = weaponObject.GetComponentInChildren<Weapon>(false);

            if (weapon.itemName.Equals(name))
                return weapon; 
        }

        return null;
    }
}
