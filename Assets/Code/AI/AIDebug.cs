using System.Collections.Generic;
using UnityEngine;

//
// The artificial intelligence debug system
// Insert into primary scene...
//

class AIDebug : MonoBehaviour
{
    public void OnGUI()
    {
        GUI.Label(new Rect(100, 100, 60, 60), "AI Debug System");
    }
}; 