using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class Catalogue : MonoBehaviour
{
    Mockup_Menu menuScript;
    SessionData sessionData;

    public Sprite radioActive;
    public Sprite radioInactive;
    public GameObject[] filterButtons;
    public GameObject productPrefab;
    public GameObject productParent;

    // ! These values below is already mapped. DO NOT ALTER THE ORDER!
    public string[] categories = {
        "Dodol",
        "Kacang",
        "Keripik",
        "Kerupuk & Kemplang",
        "Kopi",
        "Kue",
        "Lempok",
        "Manisan",
        "Sale Pisang",
        "Sambal",
        "Snack"
    };

    [System.Serializable]
    public class TempFilteredProducts
    {
        public GameObject all;
        public GameObject dodol;
        public GameObject kripik;
        public GameObject kerupuk;
        public GameObject kacang;
        public GameObject kopi;
        public GameObject snack;
    }
    public TempFilteredProducts filteredProducts;

    void Awake()
    {
        menuScript = GetComponent<Mockup_Menu>();
        if (menuScript == null)
            Debug.LogError("All global scripts needs to be inside the same object!");
        sessionData = GameObject.FindWithTag("SessionData").GetComponent<SessionData>();
    }

    void Start()
    {
        foreach (string product in sessionData.rawProducts)
        {
            CreateProductFromJSON(product);
        }
        SetCatalogHeight();
    }

    public void SetFilter(string category)
    {
        foreach (GameObject button in filterButtons)
        {
            button.transform.GetChild(1).GetComponent<Image>().sprite = radioInactive;
        }
        if (EventSystem.current.currentSelectedGameObject != null)
            EventSystem.current.currentSelectedGameObject.transform.GetChild(1).GetComponent<Image>().sprite = radioActive;

        Product[] products = productParent.GetComponentsInChildren<Product>(includeInactive:true);
        foreach (Product product in products) {
            if (category == "ALL")
                product.gameObject.SetActive(true);
            else {
                if (product.productAttr.category_code == (Array.IndexOf(categories, category)+1).ToString())
                    product.gameObject.SetActive(true);
                else
                    product.gameObject.SetActive(false);
            }
        }
        menuScript.TogglePopup();
    }

    void CreateProductFromJSON(string json)
    {
        Product.ProductAttr productAttr = JsonUtility.FromJson<Product.ProductAttr>(json);
        GameObject productObj = Instantiate(productPrefab,productPrefab.transform.position,productPrefab.transform.rotation) as GameObject;
        productObj.transform.SetParent(productParent.transform,false);
        Product product = productObj.GetComponent<Product>();
        product.productAttr = productAttr;
        product.Setup();

        Button productButton = productObj.GetComponent<Button>();
        productButton.onClick.AddListener(delegate {menuScript.OpenScene("Menu Product");});
    }

    void SetCatalogHeight() {
        GridLayoutGroup gridLayout = productParent.GetComponent<GridLayoutGroup>();
        float itemHeight = gridLayout.cellSize.y;
        int allItems = sessionData.rawProducts.Count;
        int multiplier = (allItems / 2) + (1 * (allItems % 2)) + 1;
        float tolerance = 100f;
        float finalHeight = (itemHeight * multiplier) + tolerance;

        RectTransform productParentRT = productParent.transform as RectTransform;
        productParentRT.sizeDelta = new Vector2(productParentRT.sizeDelta.x, finalHeight);
        productParentRT.anchoredPosition = new Vector2(productParentRT.anchoredPosition.x,-(finalHeight/2));
    }
}