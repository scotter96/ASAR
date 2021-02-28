using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Database;

public class SessionData : MonoBehaviour
{
    [Tooltip("Check if local storage is to be used.")]
    public bool useLocalStorage = true;

    DatabaseReference rootRef;
    FirebaseDatabase database;
    bool startInitFirebase;
    public string[] tableNames = {"product_item","users"};
    public List<string> rawProducts;
    public List<string> users;

    public List<Dictionary<string,string>> itemsInCart = new List<Dictionary<string,string>>();
    public string lastScene;
    public string currentScene;
    public string context;
    public string paymentMethod;
    
    [System.Serializable]
    public class ProductInfo
    {
        public string code;
        public string name;
        public string category_code;
        public int price;
        public int qty;
    }
    public ProductInfo productInfo;

    // * Active User
    [System.Serializable]
    public class User {
        public string email;
        public string username;
        public string password;
    }
    public User user;

    void Awake() {
        // ? Make this Game Object permanent
        DontDestroyOnLoad(gameObject);

        // ? Get the root reference location of the database.
        rootRef = FirebaseDatabase.DefaultInstance.RootReference;
        database = rootRef.Database;
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(GetDependencyResult);
    }

    void GetDependencyResult(Task<DependencyStatus> task) {
        var dependencyStatus = task.Result;
        if (dependencyStatus == Firebase.DependencyStatus.Available)
        {
            startInitFirebase = true;
        }
        else
        {
            UnityEngine.Debug.LogError(System.String.Format(
            "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            // ! Firebase Unity SDK is not safe to use here.
        }
    }

    void Update()
    {
        if (startInitFirebase) {
            foreach (string tableName in tableNames) {
                // TODO: Disabled temporarily since sometimes works sometimes not, so now just relying on ValueChanged
                // InitDBRead(
                //     tableName: tableName
                //     // knownKey: "category_code",
                //     // knownValue: "1"
                // );
                database.GetReference(tableName).ValueChanged += RealtimeDBRead;
            }
            startInitFirebase = false;
        }
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
    }

    public string GetPayment() {
        return paymentMethod;
    }

    public void SaveProductInfo (
        string codeInput,
        string nameInput,
        string catCodeInput,
        int priceInput,
        int qtyInput
    ) {
        productInfo.code = codeInput;
        productInfo.name = nameInput;
        productInfo.category_code = catCodeInput;
        productInfo.price = priceInput;
        productInfo.qty = qtyInput;
    }

    public Dictionary<string,object> GetProductInfo () {
        Dictionary<string,object> dict = new Dictionary<string, object>();
        dict.Add("code",productInfo.code);
        dict.Add("name",productInfo.name);
        dict.Add("category_code",productInfo.category_code);
        dict.Add("price",productInfo.price);
        dict.Add("qty",productInfo.qty);
        return dict;
    }

    public bool CreateUser (string email, string username, string password) {
        bool write = false;

        // ? Update the User object with parameters received
        user.email = email;
        user.username = username;
        user.password = password;

        // ? Write the new user data to database
        string json = JsonUtility.ToJson(user);
        write = DBWrite("users",json);

        // ? Flush the User object parameters
        user.email = string.Empty;
        user.username = string.Empty;
        user.password = string.Empty;

        return write;
    }

    public void LoginSuccess (string username, string password) {
        user.username = username;
        user.password = password;

        if (useLocalStorage)
            SaveToLocal();
    }

    public void LogoutSuccess () {
        ResetSave();
    }

    public void AddToCart(string code, string name, string category_code, int price, int qty) {
        bool itemExisted = false;
        foreach (Dictionary<string,string> item in itemsInCart) {
            if (item["code"] == code) {
                if (qty > 0)
                    item["qty"] = qty.ToString();
                else
                    itemsInCart.Remove(item);
                itemExisted = true;
                break;
            }
        }
        if (!itemExisted) {
            if (qty > 0) {
                Dictionary<string,string> newData = new Dictionary<string,string>();
                newData.Add("code",code);
                newData.Add("name",name);
                newData.Add("category_code",category_code);
                newData.Add("price",price.ToString());
                newData.Add("qty",qty.ToString());
                itemsInCart.Add(newData);
            }
        }
    }

    public void ExitApp() {
        if (useLocalStorage)
            SaveToLocal();
        Application.Quit();
    }

    // * Load a Sprite from Resources (e.g. Assets/Resources/Products/Cashew)
    public static Sprite GetSprite(string filename) {
        return Resources.Load<Sprite>($"Products/{filename}");
    }

    // **************** FIREBASE REALTIME DATABASE CODES ****************

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
                    if (knownKey == null) {
                        if (tableName == "product_item" && !rawProducts.Contains(json))
                            rawProducts.Add(json);
                        else if (tableName == "users" && !users.Contains(json))
                            users.Add(json);
                    }
                    else
                    {
                        // ? This is coded accordingly to my JSON format (see `database.json` file)
                        Dictionary<string, object> tempData = data.Value as Dictionary<string, object>;
                        object val;
                        if (!tempData.TryGetValue(knownKey, out val))
                            Debug.LogError($"No such key as \"{knownKey}\" !");
                        else
                        {
                            if (knownValue == null) {
                                if (tableName == "product_item" && !rawProducts.Contains(json))
                                    rawProducts.Add(json);
                                else if (tableName == "users" && !users.Contains(json))
                                    users.Add(json);
                            }
                            else
                            {
                                if (val.ToString() == knownValue.ToString()) {
                                    if (tableName == "product_item" && !rawProducts.Contains(json))
                                        rawProducts.Add(json);
                                    else if (tableName == "users" && !users.Contains(json))
                                        users.Add(json);
                                }
                            }
                        }
                    }
                }
            }
        });
    }

    void RealtimeDBRead(object sender, ValueChangedEventArgs args)
    {
        // * Error Check
        if (args.DatabaseError != null) {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        // * Fetch the datas & flush the old ones
        string tableName = args.Snapshot.Key;
        IEnumerable<DataSnapshot> datas = args.Snapshot.Children;
        if (tableName == "product_item")
            rawProducts.Clear();
        else if (tableName == "users")
            users.Clear();

        // * Insert new datas
        foreach (DataSnapshot data in datas) {
            string json = data.GetRawJsonValue();
            if (tableName == "product_item" && !rawProducts.Contains(json))
                rawProducts.Add(json);
            else if (tableName == "users" && !users.Contains(json))
                users.Add(json);
        }
    }

    // ? This function returns boolean as the result. Can be used to update the whole record, or just desired field
    bool DBWrite(string tableName, string json="", string key="", string value="") {
        string pkId = (users.Count).ToString("");
        if (json != "" && key == "") {
            rootRef.Child(tableName).Child(pkId).SetRawJsonValueAsync(json);
            return true;
        }
        // TODO WIP: Update new value to a certain key (currently unused)
        // else if (key != "" && json == "") {
        //     rootRef.Child(tableName).Child(pkId).Child(key).SetValueAsync(value);
        //     return true;
        // }
        else {
            Debug.LogError($"Error writing {tableName}! Params:\njson: {json}\nkey: {key}\nvalue: {value}");
            return false;
        }
    }

    // **************** LOCAL STORAGE CODES ****************

    public static string localStorageFilename = "saveData.asar";
    string path = "";

    void Start() {
        // ? Load datas from Local
        if (useLocalStorage) {
            path = $"{Application.persistentDataPath}/{localStorageFilename}";
            LoadFromLocal();
        }
    }

    void LoadFromLocal() {
        if (File.Exists (path)) {
            BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (path, FileMode.Open);
			SaveData data = (SaveData) bf.Deserialize (file);
			file.Close ();

            // * Start of contents to load
            itemsInCart = data.itemsInCart;
            user.email = data.email;
            user.username = data.username;
            user.password = data.password;
            // * End of contents to load
        }
        else
            Debug.LogWarning("No local save found.");
    }

    void SaveToLocal() {
        BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (path);
		SaveData data = new SaveData ();

        // * Start of contents to save
        data.itemsInCart = itemsInCart;
        data.email = user.email;
        data.username = user.username;
        data.password = user.password;
        // * End of contents to save

        bf.Serialize (file, data);
		file.Close ();
    }

    public void ResetSave() {
        itemsInCart = new List<Dictionary<string,string>>();
        user.email = string.Empty;
        user.username = string.Empty;
        user.password = string.Empty;

        if (useLocalStorage)
            SaveToLocal();
    }

    public void FlushCart() {
        itemsInCart = new List<Dictionary<string,string>>();

        if (useLocalStorage)
            SaveToLocal();
    }
}

[Serializable]
class SaveData
{
	public List<Dictionary<string,string>> itemsInCart;
    public string email;
    public string username;
    public string password;
}