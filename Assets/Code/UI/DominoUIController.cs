/*  
 * Domino UI Controller 
 * Controls the Domino UI
 */

using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class DominoUIController : MonoBehaviour
{
    const string DROPDOWN_NAME = "SelectDominoDropdown";
    const string DESCRIPTION_TEXT_NAME = "TextDominoDescription";
    const string COST_TEXT_NAME = "TextDominoCost";
    const string PLAYER_MONEY_TEXT_NAME = "TextPlayerMoney";

    TMP_Dropdown selectDominoDropdown = null;
    TMP_Text descriptionText = null;
    TMP_Text costText = null;
    TMP_Text playerMoneyText = null;

    public void Start()
    {
        selectDominoDropdown = transform.parent.transform.Find(DROPDOWN_NAME).gameObject.GetComponent<TMP_Dropdown>();
        descriptionText = transform.parent.transform.Find(DESCRIPTION_TEXT_NAME).gameObject.GetComponent<TMP_Text>();
        costText = transform.parent.transform.Find(COST_TEXT_NAME).gameObject.GetComponent<TMP_Text>();
        playerMoneyText = transform.parent.transform.Find(PLAYER_MONEY_TEXT_NAME).gameObject.GetComponent<TMP_Text>();

        if (!selectDominoDropdown)
            throw new MissingComponentException("DominoUIController::Start: Couldn't find the SelectDominoDropdown TMP_Dropdown!");
        if (!descriptionText)
            throw new MissingComponentException("DominoUIController::Start: Couldn't find the TextDominoDescription TMP_Text!");
        if (!costText)
            throw new MissingComponentException("DominoUIController::Start: Couldn't find the TextDominoCost TMP_Text!");
        if (!playerMoneyText)
            throw new MissingComponentException("DominoUIController::Start: Couldn't find the TextPlayerMoney TMP_Text!");

        List<string> options = new(); 

        foreach (Domino domino in DominoManager.dominoes)
        {
            // criteria for skipping:
                // - set already present
                // - not unlocked yet
                // - not enough money but don't do that here 
            bool skip = false;
            
            if (GameManager.player.car.HasModifierSet(domino.internalName))
                skip = true;

            if (!string.IsNullOrWhiteSpace(domino.requires)
                && !GameManager.player.car.HasModifierSet(domino.requires))
            {
                skip = true; 
            }

            if (!skip)
                options.Add(domino.name);
        }

        selectDominoDropdown.AddOptions(options);
        selectDominoDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        OnDropdownValueChanged(selectDominoDropdown.value); // ensure a default item just in case

        playerMoneyText.text = "Money: $" + GameManager.player.stats.money;
    }

    public void OnDropdownValueChanged(int index)
    {
        AudioManagerGlobalSounds.PlayUIClickSound();
        // get the domino with the name
        Domino selectedDomino = DominoManager.GetDominoByName(selectDominoDropdown.options[index].text);

        descriptionText.text = selectedDomino.description;
        costText.text = "Cost: $" + selectedDomino.cost.ToString(); 
    }

    /// <summary>
    /// Fired on the Buy button being clicked.
    /// </summary>
    public void BuyClicked()
    {
        AudioManagerGlobalSounds.PlayUIClickSound();

        Domino domino = DominoManager.GetDominoByName(selectDominoDropdown.options[selectDominoDropdown.value].text);

        if (domino == null)
        {
            Debug.LogError("Obtained INVALID domino " + selectDominoDropdown.options[selectDominoDropdown.value].text);
            return;
        }

        if (domino.cost > GameManager.player.stats.money)
        {
            costText.text = "Not enough money!";
            AudioManagerGlobalSounds.PlayUIWrongSound();
            return;
        }

        AudioManager.PlayAudioAtCameraPosition("UI_Buy", 1.0f);

        GameManager.player.stats.money -= domino.cost;
        playerMoneyText.text = "Money: $" + GameManager.player.stats.money;

        CarManager.ApplyModifierSetToPlayerCar(domino.modifiers);
    }

    /// <summary>
    /// Fired on the done button being clicked
    /// </summary>
    public void DoneClicked()
    {
        AudioManagerGlobalSounds.PlayUIClickSound();

        GameManager.SetGameState(GameManager.GameModeEnum.RaceMode);
    }
}