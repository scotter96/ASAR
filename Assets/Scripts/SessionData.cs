using UnityEngine;
using System.Collections.Generic;
using Firebase.Database;

public class SessionData : MonoBehaviour
{
    public string tableName = "product_item";
    FirebaseDatabase database;
    List<string> rawProducts;

    Dictionary<string,int>[] itemsInCart;
    string currentUsername;
    string lastScene;
    string currentScene;
    string context;
    string paymentMethod;
    
    [System.Serializable]
    public class ProductInfo
    {
        public string code;
        public string name;
        public string category_code;
        public decimal price;
        public int qty;
    }
    ProductInfo productInfo;

    void Awake() {
        //// FirebaseApp.CheckAndFixDependenciesAsync();
        // ? Get the root reference location of the database.
        database = FirebaseDatabase.DefaultInstance.RootReference.Database;
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        
        InitDBRead(
            tableName: tableName
            // knownKey: "category_code",
            // knownValue: "1"
        );
        database.GetReference(tableName).ValueChanged += RealtimeDBRead;
    }

    public void SaveSceneSession(string nextScene, string thisScene, string contextData="")
    {
        currentScene = nextScene;
        lastScene = thisScene;
        context = contextData;
    }

    public string GetContext() {
        return context;
    }

    public void SetPayment(string payment) {
        paymentMethod = payment;
        Debug.Log($"Payment set to {paymentMethod}!");
    }

    public string GetPayment() {
        return paymentMethod;
    }

    public void SaveProductInfo (
        string codeInput,
        string nameInput,
        string catCodeInput,
        decimal priceInput,
        int qtyInput
    ) {
        productInfo.code = codeInput;
        productInfo.name = nameInput;
        productInfo.category_code = catCodeInput;
        productInfo.price = priceInput;
        productInfo.qty = qtyInput;
    }

    // TODO: Get product information
    // public Dictionary<string,object> GetProductInfo () {}

    void InitDBRead(string tableName, string knownKey = null, object knownValue = null)
    {
        // ? Get the reference based on self-made `table name`
        database.GetReference(tableName).GetValueAsync().ContinueWith(task =>
        {
            // ? What to do if the task fails
            if (task.IsFaulted)
                Debug.LogError($"Failed to retrieve data from table \"{tableName}\" !");
            // ? Fetch the data
            else if (task.IsCompleted)
            {
                IEnumerable<DataSnapshot> datas = task.Result.Children;
                foreach (DataSnapshot data in datas)
                {
                    string json = data.GetRawJsonValue();
                    if (knownKey == null)
                        rawProducts.Add(json);
                    else
                    {
                        // ? This is coded accordingly to my JSON format (see `database.json` file)
                        Dictionary<string, object> tempData = data.Value as Dictionary<string, object>;
                        object val;
                        if (!tempData.TryGetValue(knownKey, out val))
                            Debug.LogError($"No such key as \"{knownKey}\" !");
                        else
                        {
                            if (knownValue == null)
                                rawProducts.Add(json);
                            else
                            {
                                if (val.ToString() == knownValue.ToString())
                                    rawProducts.Add(json);
                            }
                        }
                    }
                }
            }
        });
    }

    void RealtimeDBRead(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null) {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        IEnumerable<DataSnapshot> datas = args.Snapshot.Children;
        rawProducts.Clear();
        foreach (DataSnapshot data in datas) {
            string json = data.GetRawJsonValue();
            rawProducts.Add(json);
        }
    }
}
