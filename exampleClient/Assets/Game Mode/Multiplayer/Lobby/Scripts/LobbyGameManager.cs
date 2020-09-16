using System.Collections;
using System.Collections.Generic;
using System.Net;
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
    private float startGameCounter = 6f;

    // store all players info in the client side.
    public static Dictionary<int, User> clientsInLobby = new Dictionary<int, User>();
    private List<int> readyUsers = new List<int>();

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
        clientsInLobby.Clear();
        readyUsers = new List<int>();
        Client.instance.userName = DataBridge.instance.userProfile.username;
        Client.instance.ConnectToServer();

        panel = lobbyCanvas.transform.GetChild(0).GetComponent<Image>();
        colorPanel = panel.transform.GetChild(0).GetComponent<Image>();
        playersFrame = colorPanel.transform.GetChild(0).gameObject;
    }

    void FixedUpdate()
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
                if (user.lobbyState == "Listo" && !readyUsers.Contains(user.userServerId))
                {
                    readyUsers.Add(user.userServerId);
                }
            }
        }


        if ((readyUsers.Count == clientsInLobby.Count) && readyUsers.Count > 1)
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
                Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
                StartCoroutine(LoadAsynchronously(Client.instance.levelSelected));
                
                return;
            }
        }
    }

    public void SendToLobby(int _id, string username, string league)
    {
        if (!clientsInLobby.ContainsKey(_id))
        {
            Debug.Log("in lobby");
            clientsInLobby.Add(_id, new User(_id, username, league));
        }
    }

    IEnumerator LoadAsynchronously(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);

            Debug.Log($"LOADING {progress}");

            yield return null; // wait until next frame
        }

        if (Client.instance.hasMiddleware)
        {
            PacketSend.StartMiddleware();
        }
    }
}
