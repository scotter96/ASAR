using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Checkout : MonoBehaviour
{
    Mockup_Menu menuScript;
    SessionData sessionData;

    public GameObject cartItemPrefab;
    public GameObject cartItemsParent;
    public Text totalText;

    void Start()
    {
        menuScript = GetComponent<Mockup_Menu>();
        sessionData = GameObject.FindWithTag("SessionData").GetComponent<SessionData>();
        InitCart();
    }

    // TODO: Add scrollable panel to Cart Items on 'Checkout' scene
    void InitCart()
    {
        int totalAmount = 0;
        foreach (Dictionary<string,string> item in sessionData.itemsInCart)
        {
            // ? Create a cart item and assign it to the correct parent
            GameObject cartItem = Instantiate (cartItemPrefab, cartItemPrefab.transform.position, Quaternion.identity) as GameObject;
            cartItem.transform.SetParent(cartItemsParent.transform,false);

            // ? Create a button for said cart item
            Button itemButton = cartItem.GetComponent<Button>();
            itemButton.onClick.AddListener(delegate {menuScript.OpenScene("Menu Product");});

            // ? Assign the item's product properties
            Product cartItemProduct = cartItem.GetComponent<Product>();
            cartItemProduct.productAttr.code = item["code"];
            cartItemProduct.productAttr.name = item["name"];
            cartItemProduct.productAttr.category_code = item["category_code"];
            cartItemProduct.productAttr.price = int.Parse(item["price"]);
            cartItemProduct.productAttr.qty = int.Parse(item["stock"]);

            // ? Declare the item's UI objects
            Text itemName = cartItem.transform.GetChild(1).GetChild(0).GetComponentInChildren<Text>();
            Text itemPrice = cartItem.transform.GetChild(1).GetChild(1).GetComponentInChildren<Text>();
            Image itemImage = cartItem.transform.GetChild(0).GetComponent<Image>();

            // ? Set the item object's properties
            string name = "";
            bool getName = item.TryGetValue("name",out name);

            string price = "";
            bool getPrice = item.TryGetValue("price",out price);
            int priceInt = int.Parse(price);

            string qty = "";
            bool getQty = item.TryGetValue("qty",out qty);
            int qtyInt = int.Parse(qty);

            int subtotal = priceInt * qtyInt;
            totalAmount += subtotal;

            // ? Set the item's texts
            itemName.text = name;
            itemPrice.text = $"Rp. {price} x {qty} = Rp. {subtotal.ToString()}";

            // ? Set the item's picture
            string filename = name.Replace(" ", string.Empty);
            Sprite sprite = SessionData.GetSprite(filename);
            itemImage.sprite = sprite;
        }
        totalText.text = totalAmount.ToString();
    }
}