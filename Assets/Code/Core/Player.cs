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

    /// <summary>
    /// The prefab that the car is stored frrom - use for persistent data
    /// </summary>
    internal Car car;

    /// <summary>
    /// The actual GameObject that represents the car.
    /// </summary>
    internal Car carInWorld;

    /// <summary>
    /// The player's stats.
    /// </summary>
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
        return carInWorld != null; 
    }

    internal void DestroyCar()
    {
        GameObject.Destroy(carInWorld.gameObject);
        carInWorld = null; 
    }
}

