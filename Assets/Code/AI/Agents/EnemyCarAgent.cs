
using UnityEngine;


/// <summary>
/// Destruction derby enemy car coodinator
/// </summary>
class EnemyCarAgent : Agent
{
    /// <summary>
    /// This is the individual car that this enemy cat has
    /// </summary>
    private CarDead car;

    /// <summary>
    /// spawn this agent's car and go
    /// </summary>
    public void Spawn(CarDead carToInstantiate)
    {
        car = GameObject.Instantiate(carToInstantiate);
    }

    internal override void FixedUpdate()
    {
        base.FixedUpdate();        
    }
}