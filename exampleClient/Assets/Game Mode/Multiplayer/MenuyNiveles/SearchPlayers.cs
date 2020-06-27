using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchPlayers : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    
    async void LoadUsers(string usersearch)
    {

        var lista = await DataBridge.instance.LoadUsers(usersearch);
    }
}
