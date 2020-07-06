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
    private bool startGame = false;
    private float startGameCounter = 5f;

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
            lobbyCanvas.transform.GetChild(1).gameObject.SetActive(true);

            foreach (User user in clientsInLobby.Values)
            {
                if (user.lobbyState == "Listo")
                {
                    startGame = true;
                }
                else
                {
                    startGame = false;
                }
            }
        }
        

        if (startGame && clientsInLobby.Count > 1)
        {
            playersFrame.SetActive(false);
            lobbyCanvas.transform.GetChild(1).gameObject.SetActive(false);
            lobbyCanvas.transform.GetChild(2).gameObject.SetActive(false);
            TextMeshProUGUI countDownTimer = colorPanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

            startGameCounter -= Time.deltaTime;
            countDownTimer.gameObject.SetActive(true);
            countDownTimer.text = Mathf.FloorToInt(startGameCounter).ToString();

            if (Mathf.FloorToInt(startGameCounter) < 1)
            {
                SceneManager.LoadScene("MultiPlayer");
                PacketSend.SendIntoGame();
            }
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
