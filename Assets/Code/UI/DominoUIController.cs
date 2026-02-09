/*  
 * Domino UI Controller 
 * Controls the Domino UI
 */

using NUnit.Framework;
using TMPro;
using System.Collections.Generic;
using UnityEngine;

public class DominoUIController : MonoBehaviour
{
    const string DROPDOWN_NAME = "SelectDominoDropdown";

    TMP_Dropdown theDropdown = null; 

    public void Start()
    {
        // get the right dropdown - needs to be in SelectDominoDropdown
        TMP_Dropdown[] dropdowns = transform.parent.gameObject.GetComponentsInChildren<TMP_Dropdown>(); 

        foreach (TMP_Dropdown dropdown in dropdowns)
        {
            if (dropdown.gameObject.name == DROPDOWN_NAME)
                theDropdown = dropdown;
        }

        if (!theDropdown)
            return;

        // i am bad at programming
        if (theDropdown.options.Count > 0)
            return;

        List<string> options = new(); 

        foreach (Domino domino in DominoManager.dominoes)
        {
            options.Add(domino.dominoName);
        }

        theDropdown.AddOptions(options);    
    }

    public void DoneClicked()
    {
        Domino domino = DominoManager.GetDominoByName(theDropdown.options[theDropdown.value].text);
        
        if (domino == null)
        {
            Debug.LogError("Obtained INVALID domino " + theDropdown.options[theDropdown.value].text);
            return; 
        }

        CarManager.ApplyDominoToFirstCar(domino);   
        GameManager.SetGameState(GameManager.GameModeEnum.RaceMode);
    }
}