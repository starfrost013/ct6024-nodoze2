using System;
using System.Collections.Generic;
using UnityEngine;
//
// Entrance point for domino system. Called from GameModeInit
//
internal static class UpgradeManager
{
    internal static List<Upgrade> upgrades = new();

    internal static void Init()
    {
        if (upgrades.Count > 0)
            return;

        upgrades = new();

        TextAsset[] configFiles = AssetManager.LoadAssetsInFolder<TextAsset>("Domino");

        foreach (TextAsset configFile in configFiles)
        {
            Debug.Log("Loading domino at " + configFile.name);
            Upgrade domino = new(configFile);
            upgrades.Add(domino);
        }
    }

    internal static Upgrade GetUpgradeByName(string upgradeName)
    {
        if (upgrades.Count == 0)
            return null;

        foreach (Upgrade upgrade in upgrades)
        {
            if (upgrade.name == upgradeName) 
                return upgrade;
        }

        return null; // no matching domino
    }

    internal static Upgrade GetUpgradeByInternalName(string internalName)
    {
        if (upgrades.Count == 0)
            return null;

        foreach (Upgrade upgrade in upgrades)
        {
            if (upgrade.internalName == internalName)
                return upgrade;
        }

        return null; // no matching domino
    }


}