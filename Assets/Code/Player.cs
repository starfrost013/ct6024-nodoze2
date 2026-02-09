using System;
using UnityEngine;

/// <summary>
/// Player information. 
/// 
/// Stores the current car, the current player and so on
/// </summary>
internal class Player
{
    internal struct PlayerStats
    {
        // coolness used to buy things
        float money; 
    };

    
    internal Car car;
    internal PlayerStats stats;
    
}

