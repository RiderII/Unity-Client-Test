using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using TMPro;
using Facebook.Unity;
using Firebase.Extensions;
using UnityEngine.Networking;

public class AuthController : MonoBehaviour
{
    FirebaseAuth auth;
    private bool logged = false;
    private bool isNewUser = false;
    private bool fblogged = false;
    private bool error = false;
    private AuthError errorType;

    public TMP_InputField usernameField;
    public TMP_InputField pwdRegister;
    public TMP_InputField emailRegister;

    public TMP_InputField emailLogin;
    public TMP_InputField pwdLogin;

    public GameObject errorPanel;

    private Dictionary<string, int> intentosFallidos = new Dictionary<string, int>();

    public void Awake()
    {
        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
    }
    Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;
    public void Start()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    protected void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        auth = FirebaseAuth.DefaultInstance;
    }

    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }
    public void Update()
    {
        if (logged)
        {
            LoggedSuccess();
            logged = false;
        }
        if (fblogged)
        {
            FbLoggedSuccess();
            fblogged = false;
        }

        if (error)
        {
            error = false;
            GetErrorMessage(errorType);
        }
    }
    public void Login()
    {
            auth.SignInWithEmailAndPasswordAsync(emailLogin.text, pwdLogin.text).ContinueWith((task =>
            {
                if (task.IsCanceled)
                {
                    Firebase.FirebaseException e =
                    task.Exception.Flatten().InnerExceptions[0] as Firebase.FirebaseException;

                    error = true;
                    errorType = (AuthError)e.ErrorCode;
                    return;
                }
                if (task.IsFaulted)
                {
                    Firebase.FirebaseException e =
                    task.Exception.Flatten().InnerExceptions[0] as Firebase.FirebaseException;

                    error = true;
                    errorType = (AuthError)e.ErrorCode;
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

                    error = true;
                    errorType = (AuthError)e.ErrorCode;
                    return;
                }
                if (task.IsFaulted)
                {
                    Firebase.FirebaseException e =
                    task.Exception.Flatten().InnerExceptions[0] as Firebase.FirebaseException;

                    error = true;
                    errorType = (AuthError)e.ErrorCode;
                    return;
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
        print(msg);

        switch (errorCode)
        {
            case AuthError.EmailAlreadyInUse:
                msg = "Este correo electrónico ya esta en uso";
                break;
            case AuthError.AccountExistsWithDifferentCredentials:
                msg = "La cuenta existe con credenciales diferentes";
                break;
            case AuthError.MissingPassword:
                msg = "Ingrese una contraseña";
                break;
            case AuthError.MissingEmail:
                msg = "Ingrese un correo electrónico";
                break;
            case AuthError.WrongPassword:
                if (intentosFallidos.ContainsKey(emailLogin.text))
                {
                    if(intentosFallidos[emailLogin.text] == 1)
                    {
                        intentosFallidos[emailLogin.text] = intentosFallidos[emailLogin.text] + 1;
                        msg = "Contraseña incorrecta. Un intento fallido más bloqueará la cuenta por 30 minutos";
                    }else if(intentosFallidos[emailLogin.text] == 2)
                    {
                        intentosFallidos[emailLogin.text] = intentosFallidos[emailLogin.text] + 1;
                        msg = "Contraseña incorrecta. Se ha bloqueado su cuenta por 30 minutos";
                        StartCoroutine(GetRequest(emailLogin.text));
                    }
                }
                else
                {
                    intentosFallidos.Add(emailLogin.text, 1);
                    msg = "Contraseña incorrecta";
                }
                break;
            case AuthError.UserDisabled:
                msg = "Su cuenta ha sido deshabilitada intentelo más tarde";
                break;
            case AuthError.InvalidEmail:
                msg = "Correo electrónico inválido";
                break;
            case AuthError.UserNotFound:
                msg = "Correo electrónico no encontrado. Ingrese un correo registrado";
                break;
            case AuthError.TooManyRequests:
                msg = "Muchos intentos fallidos. Porfavor intentelo más tarde.";
                break;
            case AuthError.WeakPassword:
                msg = "La contraseña debe tener 6 carácteres como mínimo.";
                break;
        }

        errorPanel.SetActive(true);
        TextMeshProUGUI message = errorPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        message.text = msg;

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
    private void FbLoggedSuccess()
    {
        var user = FirebaseAuth.DefaultInstance.CurrentUser;
        User newuser = new User(user.UserId, user.DisplayName, user.Email);
        DataBridge.instance.SaveFbUser(newuser);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }


    //FACEBOOK AUTH

    public void SignInFacebook()
    {
        var perms = new List<string>() { "public_profile", "email" };
        FB.LogInWithReadPermissions(perms, AuthCallback);
    }
    private void AuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            // AccessToken class will have session details
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            Debug.Log(aToken.UserId);
            // Print current access token's granted permissions
            foreach (string perm in aToken.Permissions)
            {
                Debug.Log(perm);
            }
            FirebaseFbSignIn(aToken);
        }
        else
        {
            Debug.Log("User cancelled login");
        }
    }

    private void FirebaseFbSignIn(AccessToken token)
    {
        var credential = FacebookAuthProvider.GetCredential(token.TokenString);
        FirebaseAuth.DefaultInstance.SignInWithCredentialAsync(credential).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Firebase.FirebaseException e =
                    task.Exception.Flatten().InnerExceptions[0] as Firebase.FirebaseException;
                error = true;
                errorType = (AuthError)e.ErrorCode;
                return;
            }        
            if (task.IsFaulted)
            {
                Firebase.FirebaseException e =
                    task.Exception.Flatten().InnerExceptions[0] as Firebase.FirebaseException;
                error = true;
                errorType = (AuthError)e.ErrorCode;
                return;
            }
            if (task.IsCompleted)
            {
                FirebaseUser newUser = task.Result;
                Debug.Log($"User signed In: {newUser.DisplayName}, {newUser.UserId}.");
                //isNewUser = true;
                fblogged = true;
            }
        });
    }

    public void FacebookShare(string username)
    {
        FB.ShareLink(new System.Uri("https://github.com/RiderII"), username + "Ha terminado una carrera!",
            "RiderII es perfecto para ejercitar en cuarentena :D",
            new System.Uri("https://avatars0.githubusercontent.com/u/65631755?s=200&v=4"));
    }

    IEnumerator GetRequest(string email)
    {
        string uri = "https://us-central1-riderii.cloudfunctions.net/disableUser?email=" + email;
        UnityWebRequest uwr = UnityWebRequest.Get(uri);
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            Debug.Log("Received: " + uwr.downloadHandler.text);
        }
    }


}


