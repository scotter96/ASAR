using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Mockup_Menu : MonoBehaviour
{
    bool popupActive;
    SessionData sessionData;
    public string mapsUrl = "https://goo.gl/maps/LW4kPL4KFKfXmdqt6";
    
    public Text welcomeText;
    public GameObject sessionDataPrefab;
    public GameObject loadingPopup;
    public GameObject noAuthPopup;

    [System.Serializable]
    public class LoginItems {
        public GameObject noticePopup;
        public GameObject nullPopup;
    }
    public LoginItems loginItems;

    [System.Serializable]
    public class RegisterItems {
        public GameObject noticePopup;
        public GameObject nullPopup;
    }
    public RegisterItems registerItems;

    [System.Serializable]
    public class MainItems {
        public GameObject accountPopup;
    }
    public MainItems mainItems;

    [System.Serializable]
    public class CatalogItems {
        public GameObject filterPopup;
    }
    public CatalogItems catalogItems;

    [System.Serializable]
    public class AddressItems {
        public GameObject cityPopup;
        public GameObject courierPopup;
    }
    public AddressItems addressItems;

    [System.Serializable]
    public class PaymentItems {
        public GameObject[] selectionObjects;
        public Color selectedColor;
        public Color disabledColor;
        public GameObject purchaseButton;

        public string currentPayment;
    }
    public PaymentItems paymentItems;

    void Start() {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "Menu Login" && GameObject.FindWithTag("SessionData") == null) {
            GameObject sessionDataObj = Instantiate(sessionDataPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        }
        sessionData = GameObject.FindWithTag("SessionData").GetComponent<SessionData>();

        if (sceneName == "Menu Payment") {
            RecheckPaymentButton(true,null);
        }

        if (sceneName == "Menu Utama" && sessionData.currentUsername != "")
            welcomeText.text = $"Selamat Datang, {sessionData.currentUsername}!";
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
                    OpenScene("Menu Checkout");
                else if (sceneName == "Menu Product")
                    OpenScene("Menu Katalog");
                else if (sceneName == "Menu Tentang")
                    OpenScene("Menu Utama");
                else if (sceneName == "Menu Register")
                    OpenScene("Menu Login");
                else if (sceneName == "Menu Utama" || sceneName == "Menu Login")
                    ExitApp();
            }
        }
    }

    public void TogglePopup(string popup="") {
        string currentScene = SceneManager.GetActiveScene().name;

        if (popup == "Loading") {
            loadingPopup.SetActive(!loadingPopup.activeInHierarchy);
            popupActive = loadingPopup.activeInHierarchy;
        }
        else if (popup == "NoAuth") {
            noAuthPopup.SetActive(!noAuthPopup.activeInHierarchy);
            popupActive = noAuthPopup.activeInHierarchy;
        }
        else if (popup == "Logout") {
            mainItems.accountPopup.SetActive(!mainItems.accountPopup.activeInHierarchy);
            popupActive = mainItems.accountPopup.activeInHierarchy;
        }
        else if (popup == "FilterProduct") {
            catalogItems.filterPopup.SetActive(!catalogItems.filterPopup.activeInHierarchy);
            popupActive = catalogItems.filterPopup.activeInHierarchy;
            RectTransform filterPopupRT = catalogItems.filterPopup.transform.GetChild(0) as RectTransform;
            filterPopupRT.anchoredPosition = new Vector2(filterPopupRT.anchoredPosition.x,-(filterPopupRT.sizeDelta.y)/10);
        }
        else if (popup == "SelectCity") {
            addressItems.cityPopup.SetActive(!addressItems.cityPopup.activeInHierarchy);
            popupActive = addressItems.cityPopup.activeInHierarchy;
        }
        else if (popup == "SelectCourier") {
            addressItems.courierPopup.SetActive(!addressItems.courierPopup.activeInHierarchy);
            popupActive = addressItems.courierPopup.activeInHierarchy;
        }
        else if (popup == "WrongLogin") {
            loginItems.noticePopup.SetActive(!loginItems.noticePopup.activeInHierarchy);
            popupActive = loginItems.noticePopup.activeInHierarchy;
        }
        else if (popup == "UserExisted") {
            registerItems.noticePopup.SetActive(!registerItems.noticePopup.activeInHierarchy);
            popupActive = registerItems.noticePopup.activeInHierarchy;
        }
        else if (popup == "NullFields") {
            if (currentScene == "Menu Login") {
                loginItems.nullPopup.SetActive(!loginItems.nullPopup.activeInHierarchy);
                popupActive = loginItems.nullPopup.activeInHierarchy;
            }
            else if (currentScene == "Menu Register") {
                registerItems.nullPopup.SetActive(!registerItems.nullPopup.activeInHierarchy);
                popupActive = registerItems.nullPopup.activeInHierarchy;
            }
        }
        // ? Disable all popups
        else {
            // ? Disable Loading popup
            if (
                currentScene == "Menu AR"
                || currentScene == "Menu Login"
                || currentScene == "Menu Register"
                || currentScene == "Menu Utama"
            ) {
                loadingPopup.SetActive(false);
            }

            // ? Disable NoAuth popup
            if (
                currentScene == "Menu Katalog"
                || currentScene == "Menu Product"
            ) {
                noAuthPopup.SetActive(false);
            }

            // ? Disable scene-specific popups
            if (currentScene == "Menu Katalog")
                catalogItems.filterPopup.SetActive(false);
            else if (currentScene == "Menu Utama")
                mainItems.accountPopup.SetActive(false);
            else if (currentScene == "Menu Alamat") {
                addressItems.cityPopup.SetActive(false);
                addressItems.courierPopup.SetActive(false);
            }
            else if (currentScene == "Menu Login") {
                loginItems.noticePopup.SetActive(false);
                loginItems.nullPopup.SetActive(false);
            }
            else if (currentScene == "Menu Register") {
                registerItems.noticePopup.SetActive(false);
                registerItems.nullPopup.SetActive(false);
            }
            popupActive = false;
        }
    }

    public void OpenScene(string sceneName)
    {
        string thisScene = SceneManager.GetActiveScene().name;
        if (sceneName == "Menu Product") {
            GameObject clickerObj = EventSystem.current.currentSelectedGameObject;
            string selectedProductName = clickerObj.transform.GetChild(1).GetComponent<Text>().text;
            sessionData.SaveSceneSession(sceneName,thisScene,selectedProductName);

            // ? Save the Product Information
            Product.ProductAttr productAttr = clickerObj.GetComponent<Product>().productAttr;
            sessionData.SaveProductInfo(
                codeInput: productAttr.code,
                nameInput: productAttr.name,
                catCodeInput: productAttr.category_code,
                priceInput: productAttr.price,
                qtyInput: productAttr.qty
            );
        }
        else {
            if (sceneName == "AR" || sceneName == "Menu Katalog")
                TogglePopup("Loading");
            sessionData.SaveSceneSession(sceneName,thisScene);
        }

        if (sessionData.currentUsername == "" && sceneName == "Menu Checkout")
            TogglePopup("NoAuth");
        else
            SceneManager.LoadScene(sceneName);
    }

    public void ExitApp()
    {
        Application.Quit();
    }

    // ? Fill the selected variable with clicked button
    public void ChangePayment(GameObject selected)
    {
        RecheckPaymentButton(false,selected);
    }

    void RecheckPaymentButton(bool onStart=false,GameObject selected=null) {
        if (onStart) {
            paymentItems.currentPayment = sessionData.GetPayment();
            GameObject[] mainButtons = GameObject.FindGameObjectsWithTag("MainButtons");
            foreach (GameObject buttonObj in mainButtons) {
                Text objText = buttonObj.GetComponentInChildren<Text>();
                if (objText.text == paymentItems.currentPayment) {
                    selected = buttonObj;
                    break;
                }
            }
        }

        foreach (GameObject go in paymentItems.selectionObjects)
        {
            Image objImage = go.GetComponent<Image>();
            Text objText = go.GetComponentInChildren<Text>();
            if (selected) {
                if (go.name != selected.name) {
                    objImage.color = paymentItems.disabledColor;
                    objText.color = Color.black;
                }
                else {
                    objImage.color = paymentItems.selectedColor;
                    objText.color = Color.white;
                    sessionData.SetPayment(objText.text);
                }
            }
            else {
                objImage.color = paymentItems.disabledColor;
                objText.color = Color.black;
            }
        }

        paymentItems.currentPayment = sessionData.GetPayment();
        if (paymentItems.currentPayment != null && paymentItems.currentPayment != "")
            paymentItems.purchaseButton.GetComponent<Button>().interactable = true;
        else
            paymentItems.purchaseButton.GetComponent<Button>().interactable = false;   
    }

    public void AccountClick() {
        if (sessionData.currentUsername != "")
            TogglePopup("Logout");
        else
            OpenScene("Menu Login");
    }

    public void Logout() {
        sessionData.LogoutSuccess();
        OpenScene("Menu Login");
    }

    public void OpenMapsLink() {
        Application.OpenURL(mapsUrl);
    }
}