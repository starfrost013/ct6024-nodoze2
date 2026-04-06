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
    const string DESCRIPTION_TEXT_NAME = "TextDominoDescription";

    TMP_Dropdown selectDominoDropdown = null;
    TMP_Text descriptionText = null;

    public void Start()
    {
        GameObject dropdownObject = transform.parent.transform.Find(DROPDOWN_NAME).gameObject;
        selectDominoDropdown = dropdownObject.GetComponent<TMP_Dropdown>();

        if (!selectDominoDropdown)
            return;
        GameObject textObject = transform.parent.transform.Find(DESCRIPTION_TEXT_NAME).gameObject;
        descriptionText = textObject.GetComponent<TMP_Text>(); 

        List<string> options = new(); 

        foreach (Domino domino in DominoManager.dominoes)
        {
            // criteria for skipping:
                // - set already present
                // - not unlocked yet
                // - not enough money but don't do that here 
            bool skip = false;
            if (GameManager.player.car.HasModifierSet(domino.name))
                skip = true;

            if (!string.IsNullOrWhiteSpace(domino.required)
                && !GameManager.player.car.HasModifierSet(domino.required))
            {
                skip = true; 
            }

            if (!skip)
                options.Add(domino.name);
        }

        selectDominoDropdown.AddOptions(options);
        selectDominoDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        OnDropdownValueChanged(selectDominoDropdown.value); // ensure a default item just in case
    }

    public void OnDropdownValueChanged(int index)
    {
        descriptionText.text = DominoManager.dominoes[index].description;   
    }

    public void DoneClicked()
    {
        Domino domino = DominoManager.GetDominoByName(selectDominoDropdown.options[selectDominoDropdown.value].text);
        
        if (domino == null)
        {
            Debug.LogError("Obtained INVALID domino " + selectDominoDropdown.options[selectDominoDropdown.value].text);
            return; 
        }

        CarManager.ApplyModifierSetToPlayerCar(domino.modifiers);   
        GameManager.SetGameState(GameManager.GameModeEnum.RaceMode);
    }
}