using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

public class Product : MonoBehaviour
{
    Mockup_Menu menuScript;
    SessionData sessionData;
    int qty;

    [System.Serializable]
    public class ProductAttr
    {
        public string code;
        public string name;
        public string category_code;
        public int price;
        public int qty;
    }
    public ProductAttr productAttr;

    [System.Serializable]
    public class ProductProductItems
    {
        public Image productImage;
        public Text productName;
        public Text productCat;
        public Text productVariant;
        public Text productPrice;
        public Text productStock;

        public Text productQty;
    }
    public ProductProductItems productProductItems;

    [System.Serializable]
    public class ProductCatalogItems
    {
        public Image productImage;
        public Text productName;
        public Text productPrice;
    }
    public ProductCatalogItems productCatalogItems;

    public override string ToString()
    {
        string code = productAttr.code;
        string name = productAttr.name;
        return $"[{code}] {name}";
    }

    void Start()
    {
        sessionData = GameObject.FindWithTag("SessionData").GetComponent<SessionData>();
        Setup();
    }

    public void Setup()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "Menu Product") {
            menuScript = GetComponent<Mockup_Menu>();
            LoadProductDetail();
        }
        else if (sceneName == "Menu Katalog")
        {
            string filename = productAttr.name.Replace(" ", string.Empty);
            productCatalogItems.productImage.sprite = SessionData.GetSprite(filename);
            productCatalogItems.productName.text = productAttr.name;
            productCatalogItems.productPrice.text = productAttr.price.ToString("0,0");
        }
    }

    void LoadProductDetail()
    {
        // * Set the Image of the product
        string context = sessionData.GetContext();
        if (context != null)
        {
            context = context.Replace(" ", string.Empty);
            Debug.Log(context.GetType() + " " + context);
            var productSprite = SessionData.GetSprite(context);
            if (productSprite != null)
            {
                productProductItems.productImage.sprite = productSprite;
                Debug.Log(productSprite);
            }
        }
        else
            Debug.LogError(context);

        // * Get the rest of the product info (e.g. Code, Price, Qty) before setting it
        Dictionary<string,object> productInfo = sessionData.GetProductInfo();
        string[] keys = new string[] {"code","name","category_code","price","qty"};
        for (int i=0;i<keys.Length;i++) {
            object val;
            if (productInfo.TryGetValue(keys[i], out val)) {
                string outStr = val.ToString();
                if (keys[i] == "price") {
                    // ? Lets say you have an object with value of 0.39999999999999997, now convert to string.
                    // ? R format specifier gives a string that can round-trip to an identical number.  
                    // ? Without R ToString() result would be doubleAsString = "0.4"
                    productAttr.price = int.Parse(outStr);
                }
                else if (keys[i] == "qty") {
                    productAttr.qty = int.Parse(outStr);
                }
                else if (keys[i] == "code") {
                    productAttr.code = outStr;
                }
                else if (keys[i] == "name") {
                    productAttr.name = outStr;
                }
                else if (keys[i] == "category_code") {
                    productAttr.category_code = outStr;
                }
            }
            else
                Debug.LogWarning($"Value {keys[i]} is null!");
        }

        // * Get the product category
        string category = Catalogue.categories[int.Parse(productAttr.category_code)-1];

        // * Now set the product info data
        productProductItems.productName.text = productAttr.name;
        productProductItems.productCat.text = category;
        // productProductItems.productVariant.text = productAttr.category_code;
        productProductItems.productPrice.text = productAttr.price.ToString();
        productProductItems.productStock.text = productAttr.qty.ToString();

        // * Also fetch the qty of this item in Cart
        string qtyStr = "0";
        foreach (Dictionary<string,string> item in sessionData.itemsInCart) {
            if (item["code"] == productAttr.code) {
                if (item.TryGetValue("qty", out qtyStr))
                    break;
            }
        }
        qty = int.Parse(qtyStr);
        SetQty();
    }

    // * Add (or subtract) product's qty
    public void AddQty(int amount) {
        if (sessionData.user.username == "")
            menuScript.TogglePopup("NoAuth");
        else {
            qty += amount;
            sessionData.AddToCart(
                productAttr.code,
                productAttr.name,
                productAttr.category_code,
                productAttr.price,
                productAttr.qty,
                qty
            );
            if (qty>=0)
                SetQty();
            else
                qty = 0;
        }
    }

    void SetQty() {
        productProductItems.productQty.text = qty.ToString();
    }
}