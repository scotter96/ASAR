using UnityEngine;
using UnityEngine.SceneManagement;

public class Mockup_Menu : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SceneManager.GetActiveScene().name == "Menu Alamat")
                OpenScene("Menu Katalog");
            else if (SceneManager.GetActiveScene().name == "Menu AR")
                OpenScene("Menu Utama");
            else if (SceneManager.GetActiveScene().name == "AR")
                OpenScene("Menu AR");
            else if (SceneManager.GetActiveScene().name == "Menu Checkout")
                OpenScene("Menu Alamat");
            else if (SceneManager.GetActiveScene().name == "Menu Katalog")
                OpenScene("Menu Utama");
            else if (SceneManager.GetActiveScene().name == "Menu Payment")
                OpenScene("Menu Checkout");
            else if (SceneManager.GetActiveScene().name == "Menu Product")
                OpenScene("Menu Katalog");
            else if (SceneManager.GetActiveScene().name == "Menu Tentang")
                OpenScene("Menu Utama");
            else if (SceneManager.GetActiveScene().name == "Menu Utama")
                ExitApp();
        }
        
    }
    public void OpenScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ExitApp()
    {
        Application.Quit();
    }
}
