using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Catalogue : MonoBehaviour
{
    Mockup_Menu menuScript;

    public Sprite radioActive;
    public Sprite radioInactive;
    public GameObject[] filterButtons;
    public GameObject productPrefab;
    public GameObject productParent;

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
    }

    void Start()
    {
        // SetFilter("ALL");
    }

    public void SetFilter(string category)
    {
        foreach (GameObject button in filterButtons)
        {
            button.transform.GetChild(1).GetComponent<Image>().sprite = radioInactive;
        }
        if (EventSystem.current.currentSelectedGameObject != null)
            EventSystem.current.currentSelectedGameObject.transform.GetChild(1).GetComponent<Image>().sprite = radioActive;

        if (category == "ALL")
        {
            filteredProducts.all.SetActive(true);
            filteredProducts.dodol.SetActive(false);
            filteredProducts.kripik.SetActive(false);
            filteredProducts.kerupuk.SetActive(false);
            filteredProducts.kacang.SetActive(false);
            filteredProducts.kopi.SetActive(false);
            filteredProducts.snack.SetActive(false);
        }
        else if (category == "Dodol")
        {
            filteredProducts.all.SetActive(false);
            filteredProducts.dodol.SetActive(true);
            filteredProducts.kripik.SetActive(false);
            filteredProducts.kerupuk.SetActive(false);
            filteredProducts.kacang.SetActive(false);
            filteredProducts.kopi.SetActive(false);
            filteredProducts.snack.SetActive(false);
        }
        else if (category == "Kripik")
        {
            filteredProducts.all.SetActive(false);
            filteredProducts.dodol.SetActive(false);
            filteredProducts.kripik.SetActive(true);
            filteredProducts.kerupuk.SetActive(false);
            filteredProducts.kacang.SetActive(false);
            filteredProducts.kopi.SetActive(false);
            filteredProducts.snack.SetActive(false);
        }
        else if (category == "Kerupuk")
        {
            filteredProducts.all.SetActive(false);
            filteredProducts.dodol.SetActive(false);
            filteredProducts.kripik.SetActive(false);
            filteredProducts.kerupuk.SetActive(true);
            filteredProducts.kacang.SetActive(false);
            filteredProducts.kopi.SetActive(false);
            filteredProducts.snack.SetActive(false);
        }
        else if (category == "Kacang")
        {
            filteredProducts.all.SetActive(false);
            filteredProducts.dodol.SetActive(false);
            filteredProducts.kripik.SetActive(false);
            filteredProducts.kerupuk.SetActive(false);
            filteredProducts.kacang.SetActive(true);
            filteredProducts.kopi.SetActive(false);
            filteredProducts.snack.SetActive(false);
        }
        else if (category == "Kopi")
        {
            filteredProducts.all.SetActive(false);
            filteredProducts.dodol.SetActive(false);
            filteredProducts.kripik.SetActive(false);
            filteredProducts.kerupuk.SetActive(false);
            filteredProducts.kacang.SetActive(false);
            filteredProducts.kopi.SetActive(true);
            filteredProducts.snack.SetActive(false);
        }
        else if (category == "Snack")
        {
            filteredProducts.all.SetActive(false);
            filteredProducts.dodol.SetActive(false);
            filteredProducts.kripik.SetActive(false);
            filteredProducts.kerupuk.SetActive(false);
            filteredProducts.kacang.SetActive(false);
            filteredProducts.kopi.SetActive(false);
            filteredProducts.snack.SetActive(true);
        }
        else
        {
            filteredProducts.all.SetActive(false);
            filteredProducts.dodol.SetActive(false);
            filteredProducts.kripik.SetActive(false);
            filteredProducts.kerupuk.SetActive(false);
            filteredProducts.kacang.SetActive(false);
            filteredProducts.kopi.SetActive(false);
            filteredProducts.snack.SetActive(false);
        }
        menuScript.TogglePopup();
    }

    void CreateFromJSON(string json)
    {
        // product = JsonUtility.FromJson<Product>(json);
        // GameObject productObj = Instantiate(productPrefab,productPrefab.transform.position,productPrefab.transform.rotation) as GameObject;
        // productObj.transform.SetParent(productParent.transform,false);
        // productObj.AddComponent<Product>();
        // Product newProduct = productObj.GetComponent<Product>();
        // newProduct = product;
    }
}