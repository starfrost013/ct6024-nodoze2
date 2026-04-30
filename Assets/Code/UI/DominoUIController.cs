/*  
 * Domino UI Controller 
 * Controls the Domino UI
 */

using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class DominoUIController : MonoBehaviour
{
    const string DROPDOWN_NAME = "SelectDominoDropdown";
    const string DESCRIPTION_TEXT_NAME = "TextDominoDescription";
    const string COST_TEXT_NAME = "TextDominoCost";
    const string COST_MULTIPLIER_TEXT_NAME = "TextDominoCostMultiplier";
    const string PLAYER_MONEY_TEXT_NAME = "TextPlayerMoney";
    const string IMAGE_DOMINO_MASK_NAME = "ImageDominoMask";

    TMP_Dropdown selectDominoDropdown = null;
    TMP_Text descriptionText = null;
    TMP_Text costText = null;
    TMP_Text costMultiplierText = null;
    TMP_Text playerMoneyText = null;
    GameObject imageDominoMask = null; 
    RectMask2D imageDominoMaskRect = null; 

    private void UpdateDominoVisual(Domino domino)
    {
        // I mesed this up so do some stupid math to fix it

        int index = (domino.dominoValueTop * DominoVisuals.DOMINO_NUMCOLUMNS) + domino.dominoValueBottom;

        if (index >= DominoVisuals.DOMINO_TOTAL)
            return;

        DominoVisuals.TextureExtents textureExtents = DominoVisuals.dominoTextures[index];

        // thanks for making a texture atlas that is terrible guys
        int realSizeX = DominoVisuals.DOMINO_SHEET_SIZE_X;
        int realSizeY = DominoVisuals.DOMINO_SHEET_SIZE_Y;
        RectTransform rectTransform = imageDominoMask.GetComponent<RectTransform>();

        textureExtents.x = (int)(textureExtents.x * ((rectTransform.rect.width / realSizeX)));
        textureExtents.y = (int)(textureExtents.y * ((rectTransform.rect.height / realSizeY)));
        float realDominoSizeX = (float)(DominoVisuals.DOMINO_WIDTH * (rectTransform.rect.width / realSizeX));
        float realDominoSizeY = (float)(DominoVisuals.DOMINO_HEIGHT * (rectTransform.rect.height / realSizeY));

        //rectTransform.anchoredPosition = new Vector2((float)textureExtents.x, (float)textureExtents.y);
        imageDominoMaskRect.padding = new Vector4(textureExtents.x,
            rectTransform.rect.height - (textureExtents.y + realDominoSizeY), 
            rectTransform.rect.width - (textureExtents.x + realDominoSizeX), 
            textureExtents.y);
    }

    public void Start()
    {
        if (DominoManager.dominoes.Count > 0)
            return;

        CarManager.SpawnPlayerCarForStaticUse();
        GameManager.player.carInWorld.transform.position = new Vector3(0, 0, -7); // seems to look good

        selectDominoDropdown = transform.parent.transform.Find(DROPDOWN_NAME).gameObject.GetComponent<TMP_Dropdown>();
        descriptionText = transform.parent.transform.Find(DESCRIPTION_TEXT_NAME).gameObject.GetComponent<TMP_Text>();
        costText = transform.parent.transform.Find(COST_TEXT_NAME).gameObject.GetComponent<TMP_Text>();
        costMultiplierText = transform.parent.transform.Find(COST_MULTIPLIER_TEXT_NAME).gameObject.GetComponent<TMP_Text>();
        playerMoneyText = transform.parent.transform.Find(PLAYER_MONEY_TEXT_NAME).gameObject.GetComponent<TMP_Text>();
        imageDominoMask = transform.parent.transform.Find(IMAGE_DOMINO_MASK_NAME).gameObject;

        if (!selectDominoDropdown)
            throw new MissingComponentException("DominoUIController::Start: Couldn't find the SelectDominoDropdown TMP_Dropdown!");
        if (!descriptionText)
            throw new MissingComponentException("DominoUIController::Start: Couldn't find the TextDominoDescription TMP_Text!");
        if (!costText)
            throw new MissingComponentException("DominoUIController::Start: Couldn't find the TextDominoCost TMP_Text!");
        if (!costMultiplierText)
            throw new MissingComponentException("DominoUIController::Start: Couldn't find the TextDominoCostMultiplier TMP_Text!");
        if (!playerMoneyText)
            throw new MissingComponentException("DominoUIController::Start: Couldn't find the TextPlayerMoney TMP_Text!");
        if (!imageDominoMask)
            throw new MissingComponentException("DominoUIController::Start: Couldn't find the ImageDominoMask GameObject!");

        // get the rect
        imageDominoMaskRect = imageDominoMask.GetComponent<RectMask2D>();

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
            {
                // generate random numbers
                domino.dominoValueBottom = Random.Range(1, 6);
                domino.dominoValueTop = Random.Range(1, 6);

                domino.currentCostMul = domino.costMulPerDominoValue * Mathf.Pow(domino.costMulPerDominoValue, (domino.dominoValueBottom + domino.dominoValueTop));

                costMultiplierText.text = "Cost Multiplier " + domino.currentCostMul + "x\n";

                options.Add(domino.name);

            }
        }

        selectDominoDropdown.AddOptions(options);
        selectDominoDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        OnDropdownValueChanged(selectDominoDropdown.value); // ensure a default item just in case

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

        // get the domino with the name
        Domino selectedDomino = DominoManager.GetDominoByName(selectDominoDropdown.options[index].text);

        descriptionText.text = selectedDomino.description;
        costText.text = "Cost: $" + selectedDomino.cost.ToString();

        UpdateDominoVisual(selectedDomino);
    }

    /// <summary>
    /// Fired on the Buy button being clicked.
    /// </summary>
    public void BuyClicked()
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