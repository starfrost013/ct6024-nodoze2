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

    public string weaponName;

    /// <summary>
    /// cost of the weapon in the store
    /// </summary>
    public int cost;

    public int numAttacks;

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
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public override void LoadConfig()
    {
        string configFilePath = WeaponManager.WEAPON_PATH + GameUtils.GetNonCloneName(name);
        weaponTextAsset = AssetManager.LoadAsset<TextAsset>(configFilePath);
        
        ConfigParser.Parse(weaponTextAsset.text);

        weaponName = ConfigParser.GetValue("Weapon", "Name");

        if (!string.IsNullOrWhiteSpace(weaponName))
            Debug.LogWarning("Weapon::LoadConfig - no weapon name specified, you won't be able to see it");

        // all non strings
        bool success = int.TryParse(ConfigParser.GetValue("Weapon", "Cost"), out cost)
        | int.TryParse(ConfigParser.GetValue("Weapon", "NumAttacks"), out numAttacks);
        ;

        if (!success)
            Debug.LogWarning("Weapon::LoadConfig - ome weapon parameters failed to load - this may be a problem");

    }
     
    // Update is called once per frame
    void Update()
    {
        
    }

}
