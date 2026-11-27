using UnityEngine;

/// <summary>
/// AttackFactory
/// 
/// A factory for attack classes
/// </summary>
public class AttackFactory
{
    // this might be a bad idea but i think thuis is the best way to load from the INI file

    private const string ATTACK_TEST = "AttackTest";
    private const string ATTACK_ROCKET = "AttackRocket";

    public static Attack InstantiateAttack(string attackName)
    {
        switch (attackName)
        {
            case ATTACK_TEST:
                return new AttackTest();
            case ATTACK_ROCKET:
                return new AttackRocket();
        }

        Debug.LogError("AttackFactory::InstantiateAttack - attempted to instantiate invalid attack " + attackName + "!");
        return null;
    }

    
};