using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using TMPro;

public class AuthController : MonoBehaviour
{
    private bool logged = false;
    private bool isNewUser = false;
    private bool error = false;
    private AuthError errorType;

    public TMP_InputField usernameField;
    public TMP_InputField pwdRegister;
    public TMP_InputField emailRegister;

    public TMP_InputField emailLogin;
    public TMP_InputField pwdLogin;

    public GameObject errorPanel;

    public void Update()
    {
        if (logged)
        {
            LoggedSuccess();
            logged = false;
        }

        if (error)
        {
            error = false;
            GetErrorMessage(errorType);
        }
    }
    public void Login()
    {
        FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(emailLogin.text, pwdLogin.text)
            .ContinueWith((task =>
            {
                if (task.IsCanceled)
                {
                    Firebase.FirebaseException e =
                    task.Exception.Flatten().InnerExceptions[0] as Firebase.FirebaseException;

                    error = true;
                    errorType = (AuthError)e.ErrorCode;
                    //GetErrorMessage((AuthError)e.ErrorCode);
                    return;
                }
                if (task.IsFaulted)
                {
                    Firebase.FirebaseException e =
                    task.Exception.Flatten().InnerExceptions[0] as Firebase.FirebaseException;

                    error = true;
                    errorType = (AuthError)e.ErrorCode;
                    //GetErrorMessage((AuthError)e.ErrorCode);
                    //Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                    return;
                }
                if (task.IsCompleted)
                {
                    Firebase.Auth.FirebaseUser newUser = task.Result;
                    Debug.LogFormat("User signed in successfully: {0} ({1})",
                        newUser.DisplayName, newUser.UserId);
                    logged = true;
                }
            }));
    }

    public void LoginAnonymous() {
        FirebaseAuth.DefaultInstance.SignInAnonymouslyAsync().ContinueWith((task =>
        {
            if (task.IsCanceled)
            {
                Firebase.FirebaseException e =
                task.Exception.Flatten().InnerExceptions[0] as Firebase.FirebaseException;
                GetErrorMessage((AuthError)e.ErrorCode);
                return;
            }
            if (task.IsFaulted)
            {
                Firebase.FirebaseException e =
                task.Exception.Flatten().InnerExceptions[0] as Firebase.FirebaseException;
                GetErrorMessage((AuthError)e.ErrorCode);
                return;
            }
            if (task.IsCompleted)
            {
                print("User  is LOGGED as Anonymous");
            }
        }));
    }

    public void Logout() {
        if (FirebaseAuth.DefaultInstance.CurrentUser != null)
            FirebaseAuth.DefaultInstance.SignOut();
    }

    public void RegisterUser() {
        FirebaseAuth.DefaultInstance.CreateUserWithEmailAndPasswordAsync(emailRegister.text, pwdRegister.text)
            .ContinueWith((task =>
            {
                if (task.IsCanceled)
                {
                    Firebase.FirebaseException e =
                    task.Exception.Flatten().InnerExceptions[0] as Firebase.FirebaseException;

                    GetErrorMessage((AuthError)e.ErrorCode);
                }
                if (task.IsFaulted)
                {
                    Firebase.FirebaseException e =
                    task.Exception.Flatten().InnerExceptions[0] as Firebase.FirebaseException;

                    GetErrorMessage((AuthError)e.ErrorCode);
                }
                if (task.IsCompleted)
                {
                    Firebase.Auth.FirebaseUser newUser = task.Result;
                    Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                        newUser.DisplayName, newUser.UserId);
                    isNewUser = true;
                    logged = true;
                }
            }));
    }

    void GetErrorMessage(AuthError errorCode)
    {
        string msg = "";
        msg = errorCode.ToString();


        /*switch (errorCode)
        {
            case AuthError.AccountExistsWithDifferentCredentials:
                break;
            case AuthError.MissingPassword:
                break;
            case AuthError.WrongPassword:
                break;
            case AuthError.InvalidEmail:
                break;
        }*/

        errorPanel.SetActive(true);
        TextMeshProUGUI message = errorPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        message.text = msg;
       
        print(msg);
    } 

    public void LoggedSuccess()
    {
        string userid = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        if (isNewUser)
        {
            User newuser = new User(userid, usernameField.text, emailRegister.text);
            DataBridge.instance.SaveNewUser(newuser);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        }
        else
        {
            DataBridge.instance.LoadUser(userid);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        }
        print(userid);
    }

  
}
