
using UnityEngine;

internal class PauseUIController : MonoBehaviour
{
    public void OnExitClicked()
    {
        GameManager.Shutdown();
    }
}
