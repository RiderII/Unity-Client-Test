using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SearchPlayers : MonoBehaviour
{
    public GameObject userRowPrefab;
    public GameObject container;
    public GameObject searchPanel;
    public GameObject playerView;

    public TMP_InputField caloriasInput;
    public TMP_InputField tiempoInput;
    public TMP_InputField distanciaInput;

    public GameObject medalOffPrefab;
    public GameObject medalOnPrefab;


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
            GameObject viewBtn = urow.transform.GetChild(3).gameObject;

            username.GetComponent<TextMeshProUGUI>().text = u.username;
            email.GetComponent<TextMeshProUGUI>().text = u.email;
            Button view = viewBtn.GetComponent<Button>();
            view.onClick.AddListener(() => ViewPLayer(u));

        }
    }

    private async void ViewPLayer(User u)
    {
        //pass user
        searchPanel.SetActive(false);
        playerView.SetActive(true);
        var board = playerView.transform.GetChild(0).gameObject;
        foreach (Transform child in board.transform)
            Destroy(child.gameObject);
        var playerName = playerView.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject;
        TMP_InputField username = playerName.GetComponent<TMP_InputField>();
        username.text = u.username;

        //load records
        string datos = await DataBridge.instance.LoadPlayersRecords(u.ID);
        string[] words = datos.Split(',');
        caloriasInput.text = words[0];
        tiempoInput.text = words[1];
        distanciaInput.text = words[2];

        //medals
        var lista = await DataBridge.instance.LoadPlayerMedals(u.ID);
        foreach (KeyValuePair<string, MedalSprites> entry in MedalCollection.Sprites())
        {
            if (lista.Contains(entry.Key))
            {
                GameObject medal = Instantiate(medalOnPrefab, board.transform);
                medal.GetComponent<Image>().sprite = entry.Value.on;
            }
            else
            {
                GameObject medal = Instantiate(medalOffPrefab, board.transform);
                medal.GetComponent<Image>().sprite = entry.Value.off;
            }
        }
    }
    
}
