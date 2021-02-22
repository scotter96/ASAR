using UnityEngine;
using System.Collections.Generic;

public class SessionData : MonoBehaviour
{
    Dictionary<string,int>[] itemsInCart;
    string currentUsername;
    string lastScene;
    string currentScene;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
