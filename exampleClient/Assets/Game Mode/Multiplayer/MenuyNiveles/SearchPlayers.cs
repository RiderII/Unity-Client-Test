using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SearchPlayers : MonoBehaviour
{
    public GameObject userRowPrefab;
    public GameObject container;
    
    public async void SearchUsers(string usersearch)
    {
        foreach (Transform child in container.transform)
            Destroy(child.gameObject);

        var lista = await DataBridge.instance.LoadUsers(usersearch);
        foreach(var u in lista)
        {
            var urow = Instantiate(userRowPrefab, container.transform);
            GameObject username = urow.transform.GetChild(1).gameObject;
            GameObject email = urow.transform.GetChild(2).gameObject;

            username.GetComponent<TextMeshProUGUI>().text = u.username;
            email.GetComponent<TextMeshProUGUI>().text = u.email;
        }
    }

    // m_YourThirdButton.onClick.AddListener(() => ButtonClicked(42));
}
