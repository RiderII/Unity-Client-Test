using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public Image UIpanel;
    public Canvas playerCanvas;
    public TextMesh userNameText;
    private GameObject playersFrame;
    private GameObject statisticsFrame;
    private GameObject displayInfoFrame;
    private GameObject raceRankFrame;
    public GameObject lapsFrame;
    GameObject[] playerLayers = new GameObject[4];
    public List<int> playerPlacement = new List<int>();
    public GameObject pointingArrow;
    public GameObject glass;
    public GameObject alert;

    public Sprite one;
    public Sprite two;
    public Sprite three;
    public Sprite four;

    public int id;
    public int placement;
    public string username = "diego";
    public string email = "test@test.com";
    public int points = 0;
    public int steps;
    public int laps = 1;
    public int collisions;
    public float traveled_meters = 0f;
    public float burned_calories = 0f;
    public float weight = 90f;
    public float playerSpeed = 0f;
    public float totalGameTime = 0f;
    public List<Medal> medals = new List<Medal>();
    public List<MapReport> mapReport;
    //public float totalScore = 0;
    public string league = "amateur";
    public bool finishedGame = false;
    public bool reloadRequestSent = false;
    private bool isGameOver = false;
    Vector3 oldPos;
    Vector3 initialPos;
    Vector3 previousPos;
    public GameObject lastGlassRef;
    public GameObject ptArrow;

    private float gameOverTimer = 3f;

    private float gameTimer;
    private float distanceTimer;
    public float finalTime;

    public AudioClip bikeBrake;
    public AudioClip bikeBrakecollision;
    public AudioClip checkPoint;
    public static AudioSource audioBikeBrake;
    public static AudioSource audioBikeBrakeCollision;
    public AudioSource audioSourceCheckPoint;

    [SerializeField] private GameObject raceResults;

    public void Initialize(int _id, string _username)
    {
        Time.timeScale = 1;
        oldPos = transform.position;
        initialPos = transform.position;
        previousPos = transform.position;
        id = _id;
        username = _username;
        userNameText.text = username;
        Image uiPanel = Instantiate(UIpanel, playerCanvas.transform);
        playersFrame = uiPanel.transform.GetChild(0).gameObject;
        statisticsFrame = uiPanel.transform.GetChild(1).gameObject;
        displayInfoFrame = uiPanel.transform.GetChild(2).gameObject;
        raceRankFrame = uiPanel.transform.GetChild(3).gameObject;
        lapsFrame = uiPanel.transform.GetChild(4).gameObject;
        displayInfoFrame.SetActive(false);

        if (SceneManager.GetActiveScene().name != "4.6 kilómetros" && SceneManager.GetActiveScene().name != "Vaquita")
        {
            switch (Client.instance.levelSelected)
            {
                case "200 metros": lapsFrame.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Vuelta " + laps + " / " + Constants.twoHundredmeterLaps.ToString(); break;
                case "500 metros": lapsFrame.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Vuelta " + laps + " / " + Constants.fiveHundredmeterLaps.ToString(); break;
            }
        }
        else
        {
            lapsFrame.SetActive(false);
        }

        foreach (Transform child in playersFrame.transform)
        {
            Image pFrame = child.GetComponent<Image>();
            pFrame.enabled = false;
            pFrame.transform.GetChild(0).GetComponent<Image>().enabled = false;
            pFrame.transform.GetChild(1).GetComponent<TextMeshProUGUI>().enabled = false;
        }

        raceResults.SetActive(false);

        audioBikeBrake = AddAudio(false, false, 0f);
        audioBikeBrakeCollision = AddAudio(true, false, 1f);
        audioSourceCheckPoint = AddAudio(false, false, 1.0f);
    }

    public AudioSource AddAudio(bool loop, bool playAwake, float vol)
    {
        AudioSource newAudio = gameObject.AddComponent<AudioSource>();
        newAudio.loop = loop;
        newAudio.playOnAwake = playAwake;
        newAudio.volume = vol;
        return newAudio;
    }

    public void SetCollisions(int _collision)
    {
        collisions = _collision;
        CameraController.audioSourcePedalo.volume *= 0.20f;
        Debug.Log($"player {username} collided {collisions}");
    }

    public void playBrake(float _speed)
    {
        audioBikeBrake.volume = _speed * 0.1f;
        audioBikeBrake.clip = bikeBrake;
        audioBikeBrake.Play();
    }

    public void playBrakeCollision(float _speed, bool playSound)
    {
        audioBikeBrakeCollision.volume = 1f;
        audioBikeBrakeCollision.clip = bikeBrakecollision;
        audioBikeBrakeCollision.Play();
    }

    private void setPlayersPlacementHoudini(int placement, float bestPlacement)
    {
        int playerId = 0;

        foreach (PlayerManager player in GameManager.players.Values)
        {
            if (!playerPlacement.Contains(player.id))
            {
                if (player.steps * player.laps > bestPlacement)
                { 
                    bestPlacement = player.steps * player.laps;
                    playerId = player.id;
                }
            }
        }

        if (GameManager.players.ContainsKey(playerId))
        {
            GameManager.players[playerId].placement = placement;
        }

        playerPlacement.Add(playerId);

        if (GameManager.players.Count == playerPlacement.Count)
        {
            return;
        }
        else
        {
            setPlayersPlacementHoudini(++placement, 0);
        }
    }

    private void setPlayersPlacement(int placement, float bestPlacement)
    {
        int playerId = 0;

        foreach (PlayerManager player in GameManager.players.Values)
        {
            if (!playerPlacement.Contains(player.id))
            {
                if (player.transform.position.z > bestPlacement)
                {
                    bestPlacement = player.transform.position.z;
                    playerId = player.id;
                }
            }
        }

        if (GameManager.players.ContainsKey(playerId))
        {
            GameManager.players[playerId].placement = placement;
        }

        playerPlacement.Add(playerId);

        if (GameManager.players.Count == playerPlacement.Count)
        {
            return;
        }
        else
        {
            setPlayersPlacement(++placement, 0);
        }
    }

    private void FixedUpdate()
    {
        if (!isGameOver)
        {
            if (finishedGame)
            {
                //CalculateScore();
                isGameOver = true;
                finalTime = gameTimer;
                totalGameTime = finalTime;
                PacketSend.SendPlayerStatistics(this);
            }

            foreach (PlayerManager player in GameManager.players.Values)
            {
                if (player)
                {
                    Image pFrame = null;
                    try
                    {
                        pFrame = playersFrame.transform.GetChild(player.placement - 1).GetComponent<Image>();
                    }
                    catch(Exception _ex)
                    {
                        player.placement = player.id;
                        pFrame = playersFrame.transform.GetChild(player.placement - 1).GetComponent<Image>();
                    }
                    pFrame.enabled = true;
                    pFrame.transform.GetChild(0).GetComponent<Image>().enabled = true;
                    pFrame.transform.GetChild(1).GetComponent<TextMeshProUGUI>().enabled = true;
                    pFrame.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = player.username;
                    switch (player.placement)
                    {
                        case 1: pFrame.transform.GetChild(0).GetComponent<Image>().sprite = one; break;
                        case 2: pFrame.transform.GetChild(0).GetComponent<Image>().sprite = two; break;
                        case 3: pFrame.transform.GetChild(0).GetComponent<Image>().sprite = three; break;
                        case 4: pFrame.transform.GetChild(0).GetComponent<Image>().sprite = four; break;
                    }
                }
            }

            if (!finishedGame && SceneManager.GetActiveScene().name == "Vaquita")
            {
                setPlayersPlacement(1, 0);
                playerPlacement.Clear();
                if (transform.position.z - previousPos.z > 0f)
                {
                    float distancePoints = (transform.position.z - initialPos.z);
                    previousPos = transform.position;
                    if (distancePoints >= 30f)
                    {
                        points += Utils.CalculatePoints(distanceTimer);
                        distanceTimer = 0f;
                        initialPos = transform.position;
                    }
                }
            }
            else
            {
                setPlayersPlacementHoudini(1, 0);
                playerPlacement.Clear();
            }

            gameTimer += Time.deltaTime;
            distanceTimer += Time.deltaTime;

            Vector3 distanceVector = (transform.position - oldPos);
            float distanceThisFrame = distanceVector.magnitude;
            traveled_meters += distanceThisFrame;
            playerSpeed = distanceThisFrame * 30;
            oldPos = transform.position;
            burned_calories += Utils.CaloriesBurned(weight, (playerSpeed * 60) * 60);

            statisticsFrame.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Puntos: " + points;
            statisticsFrame.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Tiempo: " + Mathf.FloorToInt(gameTimer) + " s";
            statisticsFrame.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Velocidad: " + Mathf.FloorToInt((playerSpeed * 60 * 60) / 1000) + " kmph";
            statisticsFrame.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "Distancia recorrida: " + System.Math.Round(traveled_meters, 2) + " m";
            statisticsFrame.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = "Calorías: " + System.Math.Round(burned_calories, 2) + " Kcal";
            statisticsFrame.transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = "Colisiones: " + collisions;
            statisticsFrame.transform.GetChild(6).GetComponent<TextMeshProUGUI>().text = Client.instance.timeStamp;
            raceRankFrame.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Posición: " + placement;
        }
        else
        {
            if (tag != "otherPlayer")
            {
                displayInfoFrame.SetActive(true);
                displayInfoFrame.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "¡Partida \nfinalizada!";
                gameOverTimer -= Time.deltaTime;
                if (gameOverTimer <= 0f)
                {
                    playerCanvas.gameObject.SetActive(false);
                    ShowFinishDashboard();
                    updatePlayersResults();
                    //if (!reloadRequestSent)
                    //{
                    //    PacketSend.RequestGameRestart();
                    //    reloadRequestSent = true;
                    //}
                }
            }
        }
    }

    public void RestartProperties()
    {
        collisions = 0;
        traveled_meters = 0f;
        burned_calories = 0f;
        finishedGame = false;
        reloadRequestSent = false;
        gameOverTimer = 3f;
        gameTimer = 0;
        finalTime = 0;
        displayInfoFrame.SetActive(false);
    }

    //private void CalculateScore()
    //{
    //    double distance = traveled_meters;
    //    double calories = burned_calories;

    //    switch (distance)
    //    {
    //        case var _ when distance > 100: totalScore += 50; break;
    //        case var _ when distance > 200: totalScore += 100; break;
    //        case var _ when distance > 300: totalScore += 200; break;
    //    }

    //    switch (calories)
    //    {
    //        case var _ when calories > 2: totalScore += 30; break;
    //        case var _ when calories > 4: totalScore += 60; break;
    //        case var _ when calories > 10: totalScore += 90; break;
    //    }
    //}

    private void updatePlayersResults()
    {
        foreach (PlayerManager player in GameManager.players.Values)
        {
            if (player.finishedGame)
            {

                raceResults.transform.GetChild(0).transform.GetChild(0).transform.GetChild(player.placement - 1).gameObject.SetActive(true);
                playerLayers[player.placement - 1] = raceResults.transform.GetChild(0).transform.GetChild(0).transform.GetChild(player.placement - 1).gameObject;
                // playerLayers[player.placement - 1].SetActive(true);
                playerLayers[player.placement - 1].transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = player.username;
                playerLayers[player.placement - 1].transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "TIEMPO: " + Mathf.FloorToInt(player.finalTime) + " s";
                //playerLayers[player.placement - 1].transform.GetChild(0).transform.GetChild(2).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "DISTANCIA RECORRIDA: " + System.Math.Round(player.traveled_meters, 2) + " m";
                //playerLayers[player.placement - 1].transform.GetChild(0).transform.GetChild(2).transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "CALORIAS: " + System.Math.Round(player.burned_calories, 2) + " Kcal";
                //playerLayers[player.placement - 1].transform.GetChild(0).transform.GetChild(2).transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "COLISIONES: " + player.collisions;
                playerLayers[player.placement - 1].transform.GetChild(1).transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = "PUNTAJE: " + player.points;
                //playerLayers[player.placement - 1].transform.GetChild(0).transform.GetChild(2).transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = "HORA DE INICIO " + System.DateTime.Now;
                
            }
        }
    }

    private void ShowFinishDashboard()
    {
        Time.timeScale = 1;
        raceResults.SetActive(true);
        //Disable scripts that still work while timescale is set to 0
    }

    public void InstantiatePoitingArrow(Vector3 lastGlassPosition, Quaternion lastGlassRotation)
    {
        ptArrow = Instantiate(pointingArrow, new Vector3(lastGlassPosition.x,
        -8f, lastGlassPosition.z),
        Quaternion.identity);

        lastGlassRef = Instantiate(glass, new Vector3(lastGlassPosition.x,
        lastGlassPosition.y, lastGlassPosition.z),
        lastGlassRotation);
    }

    public void DeletePoitingArrow()
    {
        audioSourceCheckPoint.clip = checkPoint;
        audioSourceCheckPoint.Play();
        Destroy(ptArrow);
        Destroy(lastGlassRef);
    }
}