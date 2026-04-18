
using UnityEngine;
using TMPro;

public class CarSelectUIController : MonoBehaviour
{
    private const string NAME_TEXT_NAME = "TextCarName";
    private const string DESCRIPTION_TEXT_NAME = "TextCarDescription";

    // stuff we need
    private TMP_Text carNameText = null;
    private TMP_Text carDescriptionText = null;

    // todo: there are multiple instances of this script. so the state is duplicated...bleh, just make it static 
    private static int selectedCarId = 0;

    public void Start()
    {
        carNameText = transform.parent.transform.Find(NAME_TEXT_NAME).gameObject.GetComponent<TMP_Text>();
        carDescriptionText = transform.parent.transform.Find(DESCRIPTION_TEXT_NAME).gameObject.GetComponent<TMP_Text>();

        if (!carNameText)
            throw new MissingComponentException("CarSelectUIController::Start: Couldn't find the TextCarName TMP_Text!");
        if (!carDescriptionText)
            throw new MissingComponentException("CarSelectUIController::Start: Couldn't find the TextCarDescription TMP_Text!");

        SetupSelectedCar();
    }

    private void SetupSelectedCar()
    {
        Car carPrefab = CarManager.carObjects[selectedCarId].GetComponent<Car>();

        carNameText.text = carPrefab.friendlyName;  
        carDescriptionText.text = carPrefab.description;  

    }

    public void PrevClicked()
    {
        // don't put it in the setter because we need to do a bounds check
        selectedCarId--;

        if (selectedCarId < 0)
            selectedCarId = 0;

        SetupSelectedCar();
    }

    public void NextClicked()
    {
        // don't put it in the setter because we need to do a bounds check
        selectedCarId++;

        if (selectedCarId >= CarManager.carObjects.Length)
            selectedCarId = 0;

        SetupSelectedCar();
    }

    public void DoneClicked()
    {
        // set the player car
        CarManager.SetPlayerCar(CarManager.carObjects[selectedCarId].name);
        GameManager.SetGameState(GameManager.GameModeEnum.RaceMode);
    }
}
