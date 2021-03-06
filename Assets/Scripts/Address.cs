using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Address : MonoBehaviour
{
    Mockup_Menu menuScript;

    public GameObject[] addressButtons;
    public GameObject[] courierButtons;
    public Text selectedCity;
    public Text selectedCourier;
    public Sprite radioActive;
    public Sprite radioInactive;

    void Awake()
    {
        menuScript = GetComponent<Mockup_Menu>();
        if (menuScript == null)
            Debug.LogError("All global scripts needs to be inside the same object!");
    }

    public void SetCity(string city)
    {
        if (city != "")
        {
            foreach (GameObject button in addressButtons)
                button.transform.GetChild(1).GetComponent<Image>().sprite = radioInactive;
            if (EventSystem.current.currentSelectedGameObject != null)
                EventSystem.current.currentSelectedGameObject.transform.GetChild(1).GetComponent<Image>().sprite = radioActive;
            selectedCity.text = city;
        }
        menuScript.TogglePopup();
    }

    public void SetCourier(string courier)
    {
        if (courier != "")
        {
            foreach (GameObject button in courierButtons)
                button.transform.GetChild(1).GetComponent<Image>().sprite = radioInactive;
            if (EventSystem.current.currentSelectedGameObject != null)
                EventSystem.current.currentSelectedGameObject.transform.GetChild(1).GetComponent<Image>().sprite = radioActive;
            selectedCourier.text = courier;
        }
        menuScript.TogglePopup();
    }
}
