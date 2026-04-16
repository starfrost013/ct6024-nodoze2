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
    const string COST_TEXT_NAME = "TextDominoCost";

    TMP_Dropdown selectDominoDropdown = null;
    TMP_Text descriptionText = null;
    TMP_Text costText = null;

    public void Start()
    {
        GameObject dropdownObject = transform.parent.transform.Find(DROPDOWN_NAME).gameObject;
        selectDominoDropdown = dropdownObject.GetComponent<TMP_Dropdown>();

        if (!selectDominoDropdown)
            throw new MissingComponentException("PostRace: Couldn't find the SelectDominoDropdown TMP_Dropdown!");

        GameObject textObject = transform.parent.transform.Find(DESCRIPTION_TEXT_NAME).gameObject;
        descriptionText = textObject.GetComponent<TMP_Text>();

        if (!descriptionText)
            throw new MissingComponentException("PostRace: Couldn't find the TextDominoDescription TMP_Text!");

        GameObject costObject = transform.parent.transform.Find(COST_TEXT_NAME).gameObject;
        costText = costObject.GetComponent<TMP_Text>();

        if (!costText)
            throw new MissingComponentException("PostRace: Couldn't find the TextDominoCost TMP_Text!");

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
        costText.text = "Cost: $" + DominoManager.dominoes[index].cost.ToString(); 
    }

    public void DoneClicked()
    {
        Domino domino = DominoManager.GetDominoByName(selectDominoDropdown.options[selectDominoDropdown.value].text);
        
        if (domino == null)
        {
            Debug.LogError("Obtained INVALID domino " + selectDominoDropdown.options[selectDominoDropdown.value].text);
            return; 
        }

        if (domino.cost > GameManager.player.stats.money)
        {
            costText.text = "Not enough money!";
            return;
        }

        GameManager.player.stats.money -= domino.cost; 

        CarManager.ApplyModifierSetToPlayerCar(domino.modifiers);   
        GameManager.SetGameState(GameManager.GameModeEnum.RaceMode);
    }
}