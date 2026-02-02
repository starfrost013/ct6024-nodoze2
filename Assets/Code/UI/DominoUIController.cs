/*  
 * Domino UI Controller 
 * Controls the Domino UI
 */

using UnityEngine;

public class DominoUIController : MonoBehaviour
{
    public void TempSetBreakCar()
    {
        Domino domino = DominoManager.GetDominoByName("Break Car");
        CarManager.ApplyDominoToFirstCar(domino);
    }

    public void TempSetSpeed1()
    {
        Domino domino = DominoManager.GetDominoByName("Speed Upgrade 1");
        CarManager.ApplyDominoToFirstCar(domino);
    }

    public void TempSetSpeed2()
    {
        Domino domino = DominoManager.GetDominoByName("Speed Upgrade 2");
        CarManager.ApplyDominoToFirstCar(domino);
    }

    public void TempSetSpeed3()
    {
        Domino domino = DominoManager.GetDominoByName("Speed Upgrade 3");
        CarManager.ApplyDominoToFirstCar(domino);
    }

    public void TempSetSonicSpeed()
    {
        Domino domino = DominoManager.GetDominoByName("Sonic Mode");
        CarManager.ApplyDominoToFirstCar(domino);
    }
}