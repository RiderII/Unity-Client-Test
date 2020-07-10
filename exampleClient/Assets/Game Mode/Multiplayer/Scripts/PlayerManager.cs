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
    GameObject[] playerLayers = new GameObject[4];
    public List<int> playerPlacement = new List<int>();
    

    public Sprite one;
    public Sprite two;
    public Sprite three;
    public Sprite four;

    public int id;
    public int placement;
    public string username = "diego";
    public string email = "test@test.com";
    public int collisions;
    public float traveled_kilometers;
    public float burned_calories;
    public float totalGameTime = 0f;
    public List<Medal> medals = new List<Medal>();
    public List<MapReport> mapReport;
    public float totalScore = 0;
    public string league = "amateur";
    public bool finishedGame = false;
    public bool reloadRequestSent = false;
    private bool isGameOver = false;

    private float gameOverTimer = 3f;

    private float gameTimer;
    private float finalTime;

    [SerializeField] private GameObject raceResults;

    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;
        userNameText.text = username;
        Image uiPanel = Instantiate(UIpanel, playerCanvas.transform);
        playersFrame = uiPanel.transform.GetChild(0).gameObject;
        statisticsFrame = uiPanel.transform.GetChild(1).gameObject;
        displayInfoFrame = uiPanel.transform.GetChild(2).gameObject;
        raceRankFrame = uiPanel.transform.GetChild(3).gameObject;
        displayInfoFrame.SetActive(false);

        foreach (Transform child in playersFrame.transform)
        {
            Image pFrame = child.GetComponent<Image>();
            pFrame.enabled = false;
            pFrame.transform.GetChild(0).GetComponent<Image>().enabled = false;
            pFrame.transform.GetChild(1).GetComponent<TextMeshProUGUI>().enabled = false;
        }

        raceResults.SetActive(false);
    }

    public void SetCollisions(int _collision)
    {
        collisions = _collision;
        CameraController.audioSourcePedalo.volume *= 0.20f;
        Debug.Log($"player {username} collided {collisions}");
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
                CalculateScore();
                isGameOver = true;
                finalTime = gameTimer;
                totalGameTime = finalTime;
                traveled_kilometers = (float)System.Math.Round(transform.position.z, 2);
                burned_calories = (float)System.Math.Round(transform.position.z / 50, 2);
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

            setPlayersPlacement(1, 0);

            playerPlacement.Clear();

            gameTimer += Time.deltaTime;
            statisticsFrame.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Tiempo: " + Mathf.FloorToInt(gameTimer) + " s";
            statisticsFrame.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Distancia recorrida: " + System.Math.Round(transform.position.z, 2) + " Km";
            statisticsFrame.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Calorías: " + System.Math.Round(transform.position.z / 50, 2) + " Kcal";
            statisticsFrame.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "Colisiones: " + collisions;
            raceRankFrame.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Posición: " + placement;
        }
        else
        {
            displayInfoFrame.SetActive(true);
            displayInfoFrame.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Game over! \nTú tiempo: " + Mathf.FloorToInt(finalTime);
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

    public void RestartProperties()
    {
        collisions = 0;
        traveled_kilometers = 0f;
        burned_calories = 0f;
        finishedGame = false;
        reloadRequestSent = false;
        gameOverTimer = 3f;
        gameTimer = 0;
        finalTime = 0;
        displayInfoFrame.SetActive(false);
    }

    private void CalculateScore()
    {
        double distance = System.Math.Round(transform.position.z, 2);
        double calories = System.Math.Round(transform.position.z / 50, 2);

        switch (distance)
        {
            case var _ when distance > 100: totalScore += 50; break;
            case var _ when distance > 200: totalScore += 100; break;
            case var _ when distance > 300: totalScore += 200; break;
        }

        switch (calories)
        {
            case var _ when calories > 2: totalScore += 30; break;
            case var _ when calories > 4: totalScore += 60; break;
            case var _ when calories > 10: totalScore += 90; break;
        }
    }

    private void updatePlayersResults()
    {
        foreach (PlayerManager player in GameManager.players.Values)
        {
            if (player.finishedGame)
            {
                playerLayers[player.placement - 1] = raceResults.transform.GetChild(2).GetChild(0).GetChild(0).GetChild(player.placement - 1).gameObject;
                playerLayers[player.placement - 1].SetActive(true);
                playerLayers[player.placement - 1].transform.GetChild(0).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = player.username;
                playerLayers[player.placement - 1].transform.GetChild(0).transform.GetChild(2).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "TIEMPO: " + Mathf.FloorToInt(player.finalTime) + " s";
                playerLayers[player.placement - 1].transform.GetChild(0).transform.GetChild(2).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "DISTANCIA RECORRIDA: " + player.burned_calories + " Km";
                playerLayers[player.placement - 1].transform.GetChild(0).transform.GetChild(2).transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "CALORIAS: " + player.traveled_kilometers + " Kcal";
                playerLayers[player.placement - 1].transform.GetChild(0).transform.GetChild(2).transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "COLISIONES: " + player.collisions;
                playerLayers[player.placement - 1].transform.GetChild(0).transform.GetChild(2).transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = "PUNTAJE: " + player.totalScore;
                playerLayers[player.placement - 1].transform.GetChild(0).transform.GetChild(2).transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = "CATEGORIA:" + player.league;
            }
        }
    }

    private void ShowFinishDashboard()
    {
        Time.timeScale = 1;
        raceResults.SetActive(true);
        //Disable scripts that still work while timescale is set to 0
    }
}