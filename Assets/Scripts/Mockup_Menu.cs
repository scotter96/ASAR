using UnityEngine;
using UnityEngine.SceneManagement;

public class Mockup_Menu : MonoBehaviour
{
    bool popupActive;
    [System.Serializable]
    public class CatalogItems {
        public GameObject productParent;
        public GameObject filterPopup;
    }
    public CatalogItems catalogItems;
    [System.Serializable]
    public class AddressItems {
        public GameObject cityPopup;
    }
    public AddressItems addressItems;

    void Start() {
        if (GameObject.Find("SessionData") == null) {
            GameObject sessionData = Instantiate(new GameObject(), Vector3.zero, Quaternion.identity) as GameObject;
            sessionData.name = "SessionData";
            sessionData.AddComponent<SessionData>();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (popupActive) {
                TogglePopup();
            }
            else {
                if (SceneManager.GetActiveScene().name == "Menu Alamat")
                    OpenScene("Menu Katalog");
                else if (SceneManager.GetActiveScene().name == "Menu AR")
                    OpenScene("Menu Utama");
                else if (SceneManager.GetActiveScene().name == "AR")
                    OpenScene("Menu AR");
                else if (SceneManager.GetActiveScene().name == "Menu Checkout")
                    OpenScene("Menu Alamat");
                else if (SceneManager.GetActiveScene().name == "Menu Katalog")
                    OpenScene("Menu Utama");
                else if (SceneManager.GetActiveScene().name == "Menu Payment")
                    OpenScene("Menu Katalog");
                    // OpenScene("Menu Checkout");
                else if (SceneManager.GetActiveScene().name == "Menu Product")
                    OpenScene("Menu Katalog");
                else if (SceneManager.GetActiveScene().name == "Menu Tentang")
                    OpenScene("Menu Utama");
                else if (SceneManager.GetActiveScene().name == "Menu Utama" || SceneManager.GetActiveScene().name == "Halaman Login")
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
        else {
            string currentScene = SceneManager.GetActiveScene().name;
            // ? Disable all popups
            if (currentScene == "Menu Katalog")
                catalogItems.filterPopup.SetActive(false);
            else if (currentScene == "Menu Alamat")
                addressItems.cityPopup.SetActive(false);
            popupActive = false;
        }
    }

    public void OpenScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ExitApp()
    {
        Application.Quit();
    }
}
