using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System.Threading.Tasks;
using Firebase.Auth;

public class DataBridge : MonoBehaviour
{
    public static DataBridge instance;
    private string DATA_URL = "https://riderii.firebaseio.com/";
    private DatabaseReference dbReference;
    private Challenge data;
    public User userProfile;

    public string GetMode()
    {
        return userProfile.mode;
    }

    public void SetMode(string mode)
    {
        userProfile.mode = mode;
    }

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }


    private void Start()
    {
        print("database init");
        userProfile = new User();
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(DATA_URL);
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void SaveData() {
        data = new Challenge("Termina una carrera!", 100);
        string jsonData = JsonUtility.ToJson(data);

        dbReference.Child("Challenges2").Child("Challenge" + Random.Range(0, 100000)).SetRawJsonValueAsync(jsonData);
        //en vez de usar un range, se debe usar un id unico usando firebase.Auth
    }

    public void SaveNewUser(User user)
    {
        userProfile = user;
        string jsonData = JsonUtility.ToJson(user);
        dbReference.Child("Users").Child(user.ID).SetRawJsonValueAsync(jsonData);
    }
    public void SaveReport(MapReport report)
    {
        print("saving report");
        string jsonData = JsonUtility.ToJson(report);
        string userid = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        string key = dbReference.Child("UserRecords").Child(userid).Push().Key;
        dbReference.Child("UserRecords").Child(userid).Child(key).SetRawJsonValueAsync(jsonData);
    }
    public void SaveUserMedal(string id)
    {
        print("saving medal");
        string userid = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        dbReference.Child("UserMedals").Child(userid).Child("ID" + id).SetValueAsync(id);
    }

    public void SaveUserPreferences(string mode)
    {
        SetMode(mode);
        string userid = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        dbReference.Child("Users").Child(userid).Child("mode").SetValueAsync(mode);
        dbReference.Child("UserMedals").Child(userid).RemoveValueAsync();
        print("Modo cambiado, medallas borradas");
    }

    public void LoadUser(string userid)
    {
        FirebaseDatabase.DefaultInstance.GetReference("Users").Child(userid).GetValueAsync()
            .ContinueWith((task =>
            {
                if (task.IsCanceled)
                {

                }
                if (task.IsFaulted)
                {

                }
                if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    string userstring = snapshot.GetRawJsonValue();
                    userProfile = JsonUtility.FromJson<User>(userstring);
                }
            }));
        print("mode loaded");
    }
    public void LoadData()
    {
        FirebaseDatabase.DefaultInstance.GetReferenceFromUrl(DATA_URL).GetValueAsync()
            .ContinueWith((task => {
                if (task.IsCanceled) {

                }
                if (task.IsFaulted)
                {

                }
                if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    string challData = snapshot.GetRawJsonValue();
                    foreach(var child in snapshot.Children)
                    {
                        string t = child.GetRawJsonValue();
                        Challenge data = JsonUtility.FromJson<Challenge>(t);
                        print("El desafio es:" + data.Descripcion);
                        print("Vale " + data.Puntos + " puntos");
                    }

                    
                }
        }));
    }
    
    public async Task<List<Challenge>> LoadDataChallenges()
    {
        List<Challenge> lista = new List<Challenge>();
        string mode = GetMode();
        await FirebaseDatabase.DefaultInstance.GetReference("Challenges").Child(mode).GetValueAsync()
            .ContinueWith((task => {
                if (task.IsCanceled)
                {
                    
                }
                if (task.IsFaulted)
                {
                    
                }
                if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    string challData = snapshot.GetRawJsonValue();
                    foreach (var child in snapshot.Children)
                    {
                        string t = child.GetRawJsonValue();
                        Challenge data = JsonUtility.FromJson<Challenge>(t);
                        lista.Add(data);
                    }
                }
            }));
        print("trayendo challenges");
        return lista; 
        
    }

    public async Task<List<string>> LoadUserMedals()
    {
        List<string> lista = new List<string>();
        string userid = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        await FirebaseDatabase.DefaultInstance.GetReference("UserMedals").Child(userid).GetValueAsync()
            .ContinueWith((task => {
                if (task.IsCanceled)
                {

                }
                if (task.IsFaulted)
                {

                }
                if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    foreach (var child in snapshot.Children)
                    {
                        string t = child.Value.ToString();
                        lista.Add(t);
                    }
                }
            }));

        return lista;
    }
    public async Task<User> LoadUserProfile()
    {
        User userProfile = new User();
        string userid = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        await FirebaseDatabase.DefaultInstance.GetReference("Users").Child(userid).GetValueAsync()
            .ContinueWith((task => {
                if (task.IsCanceled)
                {

                }
                if (task.IsFaulted)
                {

                }
                if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    string profileString = snapshot.GetRawJsonValue();
                    userProfile = JsonUtility.FromJson<User>(profileString);
                }
            }));

        return userProfile;
    }

    public async Task<List<User>> LoadUsers(string usersearch)
    {
        List<User> users = new List<User>();

        await FirebaseDatabase.DefaultInstance.GetReference("Users").GetValueAsync()
           .ContinueWith((task => {
               if (task.IsCanceled)
               {

               }
               if (task.IsFaulted)
               {

               }
               if (task.IsCompleted)
               {
                   DataSnapshot snapshot = task.Result;
                   foreach(var child in snapshot.Children)
                   {
                       string profileString = child.GetRawJsonValue();
                       User user = JsonUtility.FromJson<User>(profileString);
                       users.Add(user);
                   }
               }
           }));

        return users;
    }
}
