using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System.Threading.Tasks;

public class DataBridge : MonoBehaviour
{
    private string DATA_URL = "https://riderii.firebaseio.com/";
    private DatabaseReference dbReference;
    private Challenge data;

    private void Start()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(DATA_URL);

        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        //LoadData();
    }

    public void SaveData() {
        data = new Challenge("Termina una carrera!", 100);
        string jsonData = JsonUtility.ToJson(data);

        dbReference.Child("Challenges" + Random.Range(0, 100000)).SetRawJsonValueAsync(jsonData);
        //en vez de usar un range, se debe usar un id unico usando firebase.Auth
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
        await FirebaseDatabase.DefaultInstance.GetReferenceFromUrl(DATA_URL).GetValueAsync()
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

        
        return lista; 
        
    }


}
