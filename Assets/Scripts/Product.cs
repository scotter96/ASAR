using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Product : MonoBehaviour
{
    public string code;
    public new string name;
    public string category_code;
    public decimal price;
    public int qty;

    SessionData sessionData;

    [System.Serializable]
    public class ProductItems
    {
        public Image productImage;
        public Text productName;
        public Text productDesc;
        public Text productVariant;
        public Text productPrice;
        public Text productStock;
    }
    public ProductItems productItems;

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
            string filename = name.Replace(" ", string.Empty);
            productCatalogItems.productImage.sprite = GetSprite(filename);
            productCatalogItems.productName.text = name;
            productCatalogItems.productPrice.text = price.ToString("0,0");
        }
    }

    void LoadProductDetail()
    {
        // ? Set the Image of the product
        string context = sessionData.GetContext();
        if (context != null)
        {
            context = context.Replace(" ", string.Empty);
            Debug.Log(context.GetType() + " " + context);
            var productSprite = GetSprite(context);
            if (productSprite != null)
            {
                productItems.productImage.sprite = productSprite;
                Debug.Log(productSprite);
            }
        }
        else
            Debug.LogError(context);

        // ? Set the rest of the product info (e.g. Code, Price, Qty)
        // TODO: Get product information
        // code = sessionData.productInfo.code;
        // name = sessionData.productInfo.name;
        // category_code = sessionData.productInfo.category_code;
        // price = sessionData.productInfo.price;
        // qty = sessionData.productInfo.qty;
    }

    // ? Load a Sprite from Resources (e.g. Assets/Resources/Products/Cashew)
    Sprite GetSprite(string filename) {
        return Resources.Load<Sprite>($"Products/{filename}");
    }
}