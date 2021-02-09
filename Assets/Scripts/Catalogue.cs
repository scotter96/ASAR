using Firebase;
using Firebase.Database;
using UnityEngine;
using System.Collections.Generic;

public class Catalogue: MonoBehaviour {

  public List<object> initialDatas;

  void Awake() {
    FirebaseApp.CheckAndFixDependenciesAsync();
  }

  void Start() {
    string[] selectColumns = {"name"};
    DBRead(
      tableName: "product_item",
      knownKey: "category_code",
      knownValue: "1",
      selectColumns: selectColumns
    );
    Debug.Log(initialDatas);
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