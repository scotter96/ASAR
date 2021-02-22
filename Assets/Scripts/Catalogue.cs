using Firebase;
using Firebase.Database;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class Catalogue: MonoBehaviour {

  public List<object> initialDatas;
  Mockup_Menu menuScript;

  public Sprite radioActive;
  public Sprite radioInactive;
  public GameObject[] filterButtons;
  
  [System.Serializable]
  public class TempFilteredProducts {
    public GameObject all;
    public GameObject dodol;
    public GameObject kripik;
    public GameObject kerupuk;
    public GameObject kacang;
    public GameObject kopi;
    public GameObject snack;
  }
  public TempFilteredProducts filteredProducts;

  void Awake() {
    menuScript = GetComponent<Mockup_Menu>();
    if (menuScript == null)
      Debug.LogError ("All global scripts needs to be inside the same object!");
    // ? WIP, Temporarily disabled
    // FirebaseApp.CheckAndFixDependenciesAsync();
  }

  void Start() {
    SetFilter("ALL");
    // ? WIP, Temporarily disabled
    // string[] selectColumns = {"name"};
    // DBRead(
    //   tableName: "product_item",
    //   knownKey: "category_code",
    //   knownValue: "1",
    //   selectColumns: selectColumns
    // );
    // Debug.Log(initialDatas);
  }

  public void SetFilter(string category) {
    foreach (GameObject button in filterButtons) {
      button.transform.GetChild(1).GetComponent<Image>().sprite = radioInactive;
    }
    if (EventSystem.current.currentSelectedGameObject != null)
      EventSystem.current.currentSelectedGameObject.transform.GetChild(1).GetComponent<Image>().sprite = radioActive;

    if (category == "ALL") {
      filteredProducts.all.SetActive(true);
      filteredProducts.dodol.SetActive(false);
      filteredProducts.kripik.SetActive(false);
      filteredProducts.kerupuk.SetActive(false);
      filteredProducts.kacang.SetActive(false);
      filteredProducts.kopi.SetActive(false);
      filteredProducts.snack.SetActive(false);
    }
    else if (category == "Dodol") {
      filteredProducts.all.SetActive(false);
      filteredProducts.dodol.SetActive(true);
      filteredProducts.kripik.SetActive(false);
      filteredProducts.kerupuk.SetActive(false);
      filteredProducts.kacang.SetActive(false);
      filteredProducts.kopi.SetActive(false);
      filteredProducts.snack.SetActive(false);
    }
    else if (category == "Kripik") {
      filteredProducts.all.SetActive(false);
      filteredProducts.dodol.SetActive(false);
      filteredProducts.kripik.SetActive(true);
      filteredProducts.kerupuk.SetActive(false);
      filteredProducts.kacang.SetActive(false);
      filteredProducts.kopi.SetActive(false);
      filteredProducts.snack.SetActive(false);
    }
    else if (category == "Kerupuk") {
      filteredProducts.all.SetActive(false);
      filteredProducts.dodol.SetActive(false);
      filteredProducts.kripik.SetActive(false);
      filteredProducts.kerupuk.SetActive(true);
      filteredProducts.kacang.SetActive(false);
      filteredProducts.kopi.SetActive(false);
      filteredProducts.snack.SetActive(false);
    }
    else if (category == "Kacang") {
      filteredProducts.all.SetActive(false);
      filteredProducts.dodol.SetActive(false);
      filteredProducts.kripik.SetActive(false);
      filteredProducts.kerupuk.SetActive(false);
      filteredProducts.kacang.SetActive(true);
      filteredProducts.kopi.SetActive(false);
      filteredProducts.snack.SetActive(false);
    }
    else if (category == "Kopi") {
      filteredProducts.all.SetActive(false);
      filteredProducts.dodol.SetActive(false);
      filteredProducts.kripik.SetActive(false);
      filteredProducts.kerupuk.SetActive(false);
      filteredProducts.kacang.SetActive(false);
      filteredProducts.kopi.SetActive(true);
      filteredProducts.snack.SetActive(false);
    }
    else if (category == "Snack") {
      filteredProducts.all.SetActive(false);
      filteredProducts.dodol.SetActive(false);
      filteredProducts.kripik.SetActive(false);
      filteredProducts.kerupuk.SetActive(false);
      filteredProducts.kacang.SetActive(false);
      filteredProducts.kopi.SetActive(false);
      filteredProducts.snack.SetActive(true);
    }
    else {
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

  void DBRead(string tableName, string knownKey, object knownValue=null, string[] selectColumns=null) {
    // ? Get the root reference location of the database.
    DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
    // ? Get the reference based on self-made `table name`
    reference.Database.GetReference(tableName).GetValueAsync().ContinueWith(task => {
      // ? What to do if the task fails
      if (task.IsFaulted)
        Debug.LogError($"Failed to retrieve data from table \"{tableName}\" !");
      // ? Fetch the data
      else if (task.IsCompleted) {
        IEnumerable<DataSnapshot> datas = task.Result.Children;
        foreach (DataSnapshot data in datas) {
          // ? This is coded accordingly to my JSON format (see `database.json` file)
          Dictionary<string,object> tempData = data.Value as Dictionary<string,object>;
          object val;
          if (!tempData.TryGetValue(knownKey, out val))
            Debug.LogError($"No such key as \"{knownKey}\" !");
          else {
            if (knownValue == null) {
              initialDatas.Add(val);
            }
            else {
              if (val.ToString() == knownValue.ToString()) {
                if (selectColumns == null) {
                  initialDatas.Add(val);
                }
                else {
                  Dictionary<string,object> line = new Dictionary<string, object>();
                  foreach (string key in selectColumns) {
                    object neededValue;
                    if (!tempData.TryGetValue(key, out neededValue))
                      Debug.LogError($"No such key as \"{key}\" !");
                    else {
                      line.Add(key,neededValue);
                    }
                  }
                  initialDatas.Add(line);
                }
              }
            }
          }
        }
      }
    });
  }
}