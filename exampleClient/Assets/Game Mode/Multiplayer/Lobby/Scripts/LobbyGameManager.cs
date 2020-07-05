using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyGameManager : MonoBehaviour
{
    public Canvas lobbyCanvas;
    private Image panel;
    private Image colorPanel;
    private GameObject playersFrame;
    public static LobbyGameManager instance;

    // store all players info in the client side.
    public static Dictionary<int, User> clientsInLobby = new Dictionary<int, User>();

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            //ensure that only one instance of this class exists makes sense for every single client you only have one
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    void Start()
    {
        Client.instance.userName = DataBridge.instance.userProfile.username;
        Client.instance.ConnectToServer();

        panel = lobbyCanvas.transform.GetChild(0).GetComponent<Image>();
        colorPanel = panel.transform.GetChild(0).GetComponent<Image>();
        playersFrame = colorPanel.transform.GetChild(0).gameObject;
    }

    void Update()
    {
        foreach (User user in clientsInLobby.Values)
        {
            if (user != null)
            {
                playersFrame.transform.GetChild(user.userServerId - 1).gameObject.SetActive(true);
                playersFrame.transform.GetChild(user.userServerId - 1).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = user.league;
                playersFrame.transform.GetChild(user.userServerId - 1).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = user.username;
                TextMeshProUGUI state = playersFrame.transform.GetChild(user.userServerId - 1).GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>();
                state.text = user.lobbyState;
                state.color = user.lobbyState == "Pendiente" ? Color.red : Color.green;
            }
        }

        if (clientsInLobby.Count > 1)
        {
            lobbyCanvas.transform.GetChild(2).gameObject.SetActive(true);
        }
    }

    public void SendToLobby(int _id, string username, string league)
    {
        if (!clientsInLobby.ContainsKey(_id))
        {
            clientsInLobby.Add(_id, new User(_id, username, league));
        }
    }
}
