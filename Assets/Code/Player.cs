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
        internal float money; 
    };

  
    internal Car car;
    internal PlayerStats stats; 

    public Player()
    {
        stats = new PlayerStats();
    }

    /// <summary>
    /// determines if the player has a car
    /// </summary>
    /// <returns>A boolean indicating the car status of the player</returns>
    internal bool HasCar()
    {
        return car != null; 
    }

    internal void DestroyCar()
    {
        GameObject.Destroy(car);
        car = null; 
    }
}

