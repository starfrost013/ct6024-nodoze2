using UnityEngine;

/// <summary>
/// Weapon
/// 
/// The base weapon class
/// </summary>
public class Weapon : Item
{
    private TextAsset weaponTextAsset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public override void LoadConfig()
    {
        string configFilePath = WeaponManager.WEAPON_PATH + GameUtils.GetNonCloneName(name);
        weaponTextAsset = AssetManager.LoadAsset<TextAsset>(configFilePath);
    }
     
    // Update is called once per frame
    void Update()
    {
        
    }

}
