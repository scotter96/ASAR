using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Database;

public class SessionData : MonoBehaviour
{
    DatabaseReference rootRef;
    FirebaseDatabase database;
    bool startInitFirebase;
    public string[] tableNames = {"product_item","users"};
    public List<string> rawProducts;
    public List<string> users;

    public Dictionary<string,int>[] itemsInCart;
    public string currentUsername;
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

    [System.Serializable]
    public class User {
        public string email;
        public string username;
        public string password;
    }
    public User user;

    void Awake() {
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
        // write = DBWrite("users",json);
        write = DBWrite(tableName:"users",key:"username",value:username);

        // ? Flush the User object parameters
        user.email = string.Empty;
        user.username = string.Empty;
        user.password = string.Empty;

        return write;
    }

    public void LoginSuccess (string username) {
        currentUsername = username;
    }

    public void LogoutSuccess () {
        currentUsername = string.Empty;
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
}
