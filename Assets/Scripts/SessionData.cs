using UnityEngine;
using System.Collections.Generic;

public class SessionData : MonoBehaviour
{
    Dictionary<string,int>[] itemsInCart;
    string currentUsername;
    string lastScene;
    string currentScene;
    string context;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
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
}
