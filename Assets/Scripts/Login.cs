using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
    SessionData sessionData;
    Mockup_Menu menuScript;

    public InputField emailField;
    public InputField usernameField;
    public InputField passwordField;

    bool loginPassed;
    bool registerPassed;

    void Awake() {
        menuScript = GetComponent<Mockup_Menu>();
    }

    void Update() {
        if (sessionData == null) {
            sessionData = GameObject.FindWithTag("SessionData").GetComponent<SessionData>();
            if (Time.time < 2.0f) {
                string sceneName = SceneManager.GetActiveScene().name;
                if (sceneName == "Menu Login" && sessionData.useLocalStorage && sessionData.user.username != "")
                    menuScript.OpenScene("Menu Utama");
            }
        }

        if (loginPassed)
            PostLogin();
        if (registerPassed)
            PostRegister();
    }

    public void StartLogin()
    {
        if (usernameField.text != "" && passwordField.text != "") {
            menuScript.TogglePopup("Loading");
            // ? Check credentials inputted by user
            foreach (string userData in sessionData.users) {
                SessionData.User userObj = CreateUserFromJSON(userData);
                // ? If the username & password match a record,
                if (userObj.username == usernameField.text && userObj.password == passwordField.text) {
                    // ? Then passed the login and redirected to main menu
                    loginPassed = true;
                    break;
                }
            }
            menuScript.TogglePopup();
            // ? Else, show the wrong warning popup
            if (!loginPassed)
                menuScript.TogglePopup("WrongLogin");
        }
        else
            menuScript.TogglePopup("NullFields");
    }

    public void StartRegister()
    {
        bool attemptPassed = true;

        if (usernameField.text != "" && passwordField.text != "" && emailField.text != "") {
            menuScript.TogglePopup("Loading");
            // ? Check credentials inputted by user
            foreach (string userData in sessionData.users) {
                SessionData.User userObj = CreateUserFromJSON(userData);
                // ? If the username or email match a record,
                if (userObj.username == usernameField.text || userObj.email == emailField.text) {
                    // ? Then mark this attempt as failed
                    attemptPassed = false;
                    break;
                }
            }
            // ? If indeed failed, show the warning popup
            if (!attemptPassed) {
                menuScript.TogglePopup();
                menuScript.TogglePopup("UserExisted");
            }
            else
                registerPassed = true;
        }
        // ? If one of the required fields are empty, also mark this attempt as failed
        else {
            attemptPassed = false;
            menuScript.TogglePopup("NullFields");
        }
    }

    void PostLogin() {
        sessionData.LoginSuccess(
            usernameField.text,
            passwordField.text
        );
        menuScript.OpenScene("Menu Utama");
    }

    void PostRegister() {
        // ? When the data is written on database, redirect to Login screen
        if (sessionData.CreateUser(
            username: usernameField.text,
            password: passwordField.text,
            email: emailField.text
        ))
            menuScript.OpenScene("Menu Login");
    }

    SessionData.User CreateUserFromJSON(string json)
    {
        SessionData.User user = JsonUtility.FromJson<SessionData.User>(json);
        return user;
    }
}