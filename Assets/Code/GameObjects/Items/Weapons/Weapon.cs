using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

/// <summary>
/// Weapon
/// 
/// The base weapon class
/// </summary>
public class Weapon : Item
{
    private TextAsset weaponTextAsset;

    /// <summary>
    /// cost of the weapon in the store
    /// </summary>
    internal int cost;

    /// <summary>
    /// number of attacks this weapon has
    /// </summary>
    internal int numAttacks;

    /// <summary>
    /// maximum ammo
    /// </summary>
    internal int ammoMax;

    /// <summary>
    /// maga mmo
    /// </summary>
    internal int ammoMag;

    /// <summary>
    /// reload time
    /// </summary>
    internal float reloadTime;

    protected List<Attack> attacks = new();

    /// <summary>
    /// "general" weapon state enum shared between all weapons
    /// TODO: may get grid of this
    /// </summary>
    public enum WeaponState
    {
        Inactive = 0,
        Active = 1,
        Fired = 2,
    };

    internal WeaponState state; 


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void LoadConfig()
    {
        string configFilePath = WeaponManager.WEAPON_PATH + GameUtils.GetNonCloneName(name);
        weaponTextAsset = AssetManager.LoadAsset<TextAsset>(configFilePath);
        
        ConfigParser.Parse(weaponTextAsset.text);

        itemName = ConfigParser.GetValue("Weapon", "Name");
        description = ConfigParser.GetValue("Weapon", "Description");

        if (!string.IsNullOrWhiteSpace(itemName))
            Debug.LogWarning("Weapon::LoadConfig - no weapon name specified, you won't be able to see it");

        // all non strings
        bool success = int.TryParse(ConfigParser.GetValue("Weapon", "Cost"), out cost)
        | int.TryParse(ConfigParser.GetValue("Weapon", "NumAttacks"), out numAttacks)
        | int.TryParse(ConfigParser.GetValue("Weapon", "AmmoMax"), out ammoMax)
        | int.TryParse(ConfigParser.GetValue("Weapon", "AmmoMag"), out ammoMag)
        | float.TryParse(ConfigParser.GetValue("Weapon", "ReloadTime"), out reloadTime)
        ;

        if (!success)
            Debug.LogWarning("Weapon::LoadConfig - some weapon parameters failed to load - this may be a problem");

    }
     
    // Update is called once per frame
    void Update()
    {
        
    }

    internal virtual void Fire()
    {
        
    }

}
