
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

    private int tickCtr = 0;

    /// <summary>
    /// spawn this agent's car and go
    /// </summary>
    public void Spawn(GameObject carToInstantiate)
    {
        car = CarDead.GetUndeadCloneOfCar(carToInstantiate);
    }

    internal override void FixedUpdate()
    {
        base.FixedUpdate();

        if ((tickCtr / 20) % 2 == 0)
            car.ApplyInputMove(true, false, false, false);
        else
            car.ApplyInputMove(false, true, false, false);

        tickCtr++;
    }
}