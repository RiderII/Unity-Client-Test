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
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(DATA_URL);

        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
       
    }

    public void SaveData() {
        data = new Challenge("Termina una carrera!", 100);
        string jsonData = JsonUtility.ToJson(data);

        dbReference.Child("Challenges").Child("Challenge" + Random.Range(0, 100000)).SetRawJsonValueAsync(jsonData);
        //en vez de usar un range, se debe usar un id unico usando firebase.Auth
    }

    public void SaveReport(MapReport report)
    {
        print("saving report");
        string jsonData = JsonUtility.ToJson(report);
        string userid = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        string key = dbReference.Child("UserRecords").Child(userid).Push().Key;
        dbReference.Child("UserRecords").Child(userid).Child(key).SetRawJsonValueAsync(jsonData);
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
        await FirebaseDatabase.DefaultInstance.GetReference("Challenges").GetValueAsync()
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
}
