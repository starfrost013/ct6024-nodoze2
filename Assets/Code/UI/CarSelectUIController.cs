
using UnityEngine;
using TMPro;

public class CarSelectUIController : MonoBehaviour
{
    private const string NAME_TEXT_NAME = "TextCarName";
    private const string DESCRIPTION_TEXT_NAME = "TextCarDescription";
    private const string HANDLING_TEXT_NAME = "TextCarHandling";
    private const string SPEED_TEXT_NAME = "TextCarSpeed";
    private const string RELIABILITY_TEXT_NAME = "TextCarReliability";

    // stuff we need
    private TMP_Text carNameText = null;
    private TMP_Text carDescriptionText = null;
    private TMP_Text carHandlingText = null;
    private TMP_Text carSpeedText = null;
    private TMP_Text carReliabilityText = null;

    // todo: there are multiple instances of this script. so the state is duplicated...bleh, just make it static 
    private static int selectedCarId = 0;

    public void Start()
    {
        carNameText = transform.parent.transform.Find(NAME_TEXT_NAME).gameObject.GetComponent<TMP_Text>();
        carDescriptionText = transform.parent.transform.Find(DESCRIPTION_TEXT_NAME).gameObject.GetComponent<TMP_Text>();
        carHandlingText = transform.parent.transform.Find(HANDLING_TEXT_NAME).gameObject.GetComponent<TMP_Text>();
        carSpeedText = transform.parent.transform.Find(SPEED_TEXT_NAME).gameObject.GetComponent<TMP_Text>();
        carReliabilityText = transform.parent.transform.Find(RELIABILITY_TEXT_NAME).gameObject.GetComponent<TMP_Text>();

        if (!carNameText)
            throw new MissingComponentException("CarSelectUIController::Start: Couldn't find the TextCarName TMP_Text!");
        if (!carDescriptionText)
            throw new MissingComponentException("CarSelectUIController::Start: Couldn't find the TextCarDescription TMP_Text!");
        if (!carHandlingText)
            throw new MissingComponentException("CarSelectUIController::Start: Couldn't find the TextCarHandling TMP_Text!");
        if (!carSpeedText)
            throw new MissingComponentException("CarSelectUIController::Start: Couldn't find the TextCarSpeed TMP_Text!");
        if (!carReliabilityText)
            throw new MissingComponentException("CarSelectUIController::Start: Couldn't find the TextCarReliability TMP_Text!");

        SetupSelectedCar();
    }

    private void SetupSelectedCar()
    {
        Car carPrefab = CarManager.carObjects[selectedCarId].GetComponent<Car>();

        carNameText.text = carPrefab.metadata.friendlyName;  
        carDescriptionText.text = carPrefab.metadata.description;
        carHandlingText.text = "Handling: " + carPrefab.metadata.handlingText;
        carSpeedText.text = "Speed: " + carPrefab.metadata.speedText;
        carReliabilityText.text = "Reliability: " + carPrefab.metadata.reliabilityText;

        CarManager.SetPlayerCar(carPrefab.name);

        CarManager.SpawnPlayerCarForStaticUse();
        GameManager.player.carInWorld.transform.position = new Vector3(0, 0, -7); // seems to look good
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

    public void PrevClicked()
    {
        AudioManagerGlobalSounds.PlayUIClickSound();

        // don't put it in the setter because we need to do a bounds check
        selectedCarId--;

        if (selectedCarId < 0)
            selectedCarId = 0;

        SetupSelectedCar();
    }

    public void NextClicked()
    {
        AudioManagerGlobalSounds.PlayUIClickSound();

        // don't put it in the setter because we need to do a bounds check
        selectedCarId++;

        if (selectedCarId >= CarManager.carObjects.Length)
            selectedCarId = 0;

        Debug.Log("car id is now " + selectedCarId);
        SetupSelectedCar();
    }

    public void DoneClicked()
    {
        AudioManagerGlobalSounds.PlayUIClickSound();

        // set the player car
        CarManager.SetPlayerCar(CarManager.carObjects[selectedCarId].name);
        GameManager.SetGameState(GameManager.GameModeEnum.RaceMode);
    }
}
