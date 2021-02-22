using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Mockup_Menu : MonoBehaviour
{
    bool popupActive;
    SessionData sessionData;
    
    public GameObject sessionDataPrefab;

    [System.Serializable]
    public class CatalogItems {
        public GameObject productParent;
        public GameObject filterPopup;
    }
    public CatalogItems catalogItems;
    [System.Serializable]
    public class AddressItems {
        public GameObject cityPopup;
        public GameObject courierPopup;
    }
    public AddressItems addressItems;

    void Start() {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "Halaman Login" && GameObject.Find("SessionData") == null) {
            GameObject sessionDataObj = Instantiate(sessionDataPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        }
        sessionData = GameObject.FindWithTag("SessionData").GetComponent<SessionData>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (popupActive) {
                TogglePopup();
            }
            else {
                string sceneName = SceneManager.GetActiveScene().name;
                if (sceneName == "Menu Alamat")
                    OpenScene("Menu Checkout");
                else if (sceneName == "Menu AR")
                    OpenScene("Menu Utama");
                else if (sceneName == "AR")
                    OpenScene("Menu AR");
                else if (sceneName == "Menu Checkout")
                    OpenScene("Menu Katalog");
                else if (sceneName == "Menu Katalog")
                    OpenScene("Menu Utama");
                else if (sceneName == "Menu Payment")
                    OpenScene("Menu Alamat");
                    // OpenScene("Menu Checkout");
                else if (sceneName == "Menu Product")
                    OpenScene("Menu Katalog");
                else if (sceneName == "Menu Tentang")
                    OpenScene("Menu Utama");
                else if (sceneName == "Menu Utama" || sceneName == "Halaman Login")
                    ExitApp();
            }
        }
        
    }

    public void TogglePopup(string popup="") {
        if (popup == "FilterProduct") {
            catalogItems.filterPopup.SetActive(!catalogItems.filterPopup.activeInHierarchy);
            popupActive = catalogItems.filterPopup.activeInHierarchy;
        }
        else if (popup == "SelectCity") {
            addressItems.cityPopup.SetActive(!addressItems.cityPopup.activeInHierarchy);
            popupActive = addressItems.cityPopup.activeInHierarchy;
        }
        else if (popup == "SelectCourier") {
            addressItems.courierPopup.SetActive(!addressItems.courierPopup.activeInHierarchy);
            popupActive = addressItems.courierPopup.activeInHierarchy;
        }
        else {
            string currentScene = SceneManager.GetActiveScene().name;
            // ? Disable all popups
            if (currentScene == "Menu Katalog")
                catalogItems.filterPopup.SetActive(false);
            else if (currentScene == "Menu Alamat") {
                addressItems.cityPopup.SetActive(false);
                addressItems.courierPopup.SetActive(false);
            }
            popupActive = false;
        }
    }

    public void OpenScene(string sceneName)
    {
        string thisScene = SceneManager.GetActiveScene().name;
        if (sceneName == "Menu Product") {
            string selectedProductName = EventSystem.current.currentSelectedGameObject.transform.GetChild(1).GetComponent<Text>().text;
            sessionData.SaveSceneSession(sceneName,thisScene,selectedProductName);
        }
        else
            sessionData.SaveSceneSession(sceneName,thisScene);
        SceneManager.LoadScene(sceneName);
    }

    public void ExitApp()
    {
        Application.Quit();
    }
}
