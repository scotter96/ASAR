using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Product : MonoBehaviour
{
    SessionData sessionData;

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
    }

    public void Setup()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "Menu Product")
            LoadProductDetail();
        else if (sceneName == "Menu Katalog")
        {
            string filename = productAttr.name.Replace(" ", string.Empty);
            productCatalogItems.productImage.sprite = GetSprite(filename);
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
            var productSprite = GetSprite(context);
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

        // * Now set the product info data
        productProductItems.productName.text = productAttr.code;
        productProductItems.productCat.text = productAttr.name;
        productProductItems.productVariant.text = productAttr.category_code;
        productProductItems.productPrice.text = productAttr.price.ToString();
        productProductItems.productStock.text = productAttr.qty.ToString();
    }

    // * Load a Sprite from Resources (e.g. Assets/Resources/Products/Cashew)
    Sprite GetSprite(string filename) {
        return Resources.Load<Sprite>($"Products/{filename}");
    }
}