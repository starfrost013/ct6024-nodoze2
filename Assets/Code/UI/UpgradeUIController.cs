/*  
 * Upgrade UI Controller 
 * Controls the Upgrade UI
 */

using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UpgradUIController : MonoBehaviour
{
    const string DROPDOWN_NAME = "SelectUpgradeDropdown";
    const string DESCRIPTION_TEXT_NAME = "TextUpgradeDescription";
    const string COST_TEXT_NAME = "TextUpgradeCost";
    const string PLAYER_MONEY_TEXT_NAME = "TextPlayerMoney";

    TMP_Dropdown selectUpgradeDropdown = null;
    TMP_Text descriptionText = null;
    TMP_Text costText = null;
    TMP_Text playerMoneyText = null;

    public void Start()
    {

        CarManager.SpawnPlayerCarForStaticUse();
        GameManager.player.carInWorld.transform.position = new Vector3(0, 0, -7); // seems to look good

        selectUpgradeDropdown = transform.parent.transform.Find(DROPDOWN_NAME).gameObject.GetComponent<TMP_Dropdown>();
        descriptionText = transform.parent.transform.Find(DESCRIPTION_TEXT_NAME).gameObject.GetComponent<TMP_Text>();
        costText = transform.parent.transform.Find(COST_TEXT_NAME).gameObject.GetComponent<TMP_Text>();
        playerMoneyText = transform.parent.transform.Find(PLAYER_MONEY_TEXT_NAME).gameObject.GetComponent<TMP_Text>();

        if (!selectUpgradeDropdown)
            throw new MissingComponentException("UpgradeUIController::Start: Couldn't find the SelectUpgradeDropdown TMP_Dropdown!");
        if (!descriptionText)
            throw new MissingComponentException("UpgradeUIController::Start: Couldn't find the TextUpgradeDescription TMP_Text!");
        if (!costText)
            throw new MissingComponentException("UpgradeUIController::Start: Couldn't find the TextUpgradeCost TMP_Text!");
        if (!playerMoneyText)
            throw new MissingComponentException("UpgradeUIController::Start: Couldn't find the TextPlayerMoney TMP_Text!");

        if (selectUpgradeDropdown.options.Count > 0)
            return;

        List<string> options = new(); 

        foreach (Upgrade upgrade in UpgradeManager.upgrades)
        {
            // criteria for skipping:
                // - set already present
                // - not unlocked yet
                // - not enough money but don't do that here 
            bool skip = false;
            
            if (GameManager.player.car.HasModifierSet(upgrade.internalName))
                skip = true;

            if (!string.IsNullOrWhiteSpace(upgrade.requires)
                && !GameManager.player.car.HasModifierSet(upgrade.requires))
            {
                skip = true; 
            }

            if (!skip)
                options.Add(upgrade.name);
        }

        selectUpgradeDropdown.AddOptions(options);
        selectUpgradeDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        OnDropdownValueChanged(selectUpgradeDropdown.value); // ensure a default item just in case

        playerMoneyText.text = "Money: $" + GameManager.player.stats.money;
    }

    private void FixedUpdate()
    {
        // rotate a bit
        GameManager.player.carInWorld.transform.localEulerAngles = new Vector3(
            GameManager.player.carInWorld.transform.localEulerAngles.x,
            GameManager.player.carInWorld.transform.localEulerAngles.y + 0.5f,
            GameManager.player.carInWorld.transform.localEulerAngles.z
            );
    }

    public void OnDropdownValueChanged(int index)
    {
        AudioManagerGlobalSounds.PlayUIClickSound();

        // get the upgrade with the name
        Upgrade upgrade = UpgradeManager.GetUpgradeByName(selectUpgradeDropdown.options[index].text);

        descriptionText.text = upgrade.description;
        costText.text = "Cost: $" + upgrade.cost.ToString();
    }

    /// <summary>
    /// Fired on the Buy button being clicked.
    /// </summary>
    public void BuyClicked()
    {
        Upgrade upgrade = UpgradeManager.GetUpgradeByName(selectUpgradeDropdown.options[selectUpgradeDropdown.value].text);

        if (upgrade == null)
        {
            Debug.LogError("Obtained INVALID upgrade " + selectUpgradeDropdown.options[selectUpgradeDropdown.value].text);
            return;
        }

        if (upgrade.cost > GameManager.player.stats.money)
        {
            costText.text = "Not enough money!";
            AudioManagerGlobalSounds.PlayUIWrongSound();
            return;
        }

        AudioManager.PlayAudioAtCameraPosition("UI_Buy", 1.0f);

        GameManager.player.stats.money -= upgrade.cost;
        playerMoneyText.text = "Money: $" + GameManager.player.stats.money;

        CarManager.ApplyModifierSetToPlayerCar(upgrade.modifiers);
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