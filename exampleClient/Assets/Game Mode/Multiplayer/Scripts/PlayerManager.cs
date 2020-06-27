﻿using System.Collections;
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
    public List<int> playerPosition = new List<int>();

    public Sprite one;
    public Sprite two;
    public Sprite three;
    public Sprite four;

    public int id;
    public int position;
    public string username;
    public int collisions;
    public float traveled_kilometers;
    public float burned_calories;
    public bool finishedGame = false;
    public bool reloadRequestSent = false;

    private float gameOverTimer = 3f;

    private float gameTimer;
    private float finalTime;

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
    }

    public void SetCollisions(int _collision)
    {
        collisions = _collision;
        CameraController.audioSourcePedalo.volume *= 0.20f;
        Debug.Log($"player {username} collided {collisions}");
    }

    private void setPlayersPosition(int position, float bestPosition)
    {
        int playerId = 0;

        foreach (PlayerManager player in GameManager.players.Values)
        {
            if (!playerPosition.Contains(player.id))
            {
                if (player.transform.position.z > bestPosition)
                {
                    bestPosition = player.transform.position.z;
                    playerId = player.id;
                }
            }
        }

        if (GameManager.players.ContainsKey(playerId))
        {
            GameManager.players[playerId].position = position;
        }

        playerPosition.Add(playerId);

        if (GameManager.players.Count == playerPosition.Count)
        {
            return;
        }
        else
        {
            setPlayersPosition(++position, 0);
        }
    }

    private void Update()
    {
        foreach (PlayerManager player in GameManager.players.Values)
        {
            if (player)
            {
                Image pFrame = playersFrame.transform.GetChild(player.id - 1).GetComponent<Image>();
                pFrame.enabled = true;
                pFrame.transform.GetChild(0).GetComponent<Image>().enabled = true;
                pFrame.transform.GetChild(1).GetComponent<TextMeshProUGUI>().enabled = true;
                pFrame.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = player.username;
                switch (player.position)
                {
                    case 1: pFrame.transform.GetChild(0).GetComponent<Image>().sprite = one; break;
                    case 2: pFrame.transform.GetChild(0).GetComponent<Image>().sprite = two; break;
                    case 3: pFrame.transform.GetChild(0).GetComponent<Image>().sprite = three; break;
                    case 4: pFrame.transform.GetChild(0).GetComponent<Image>().sprite = four; break;
                }
            }
        }

        setPlayersPosition(1, 0);

        playerPosition.Clear();

        if (finishedGame)
        {
            finalTime = gameTimer;
            displayInfoFrame.SetActive(true);
            displayInfoFrame.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Game over! \nTú tiempo: " + Mathf.FloorToInt(finalTime);
            gameOverTimer -= Time.deltaTime;
            if (gameOverTimer <= 0f)
            {
                if (!reloadRequestSent)
                {
                    PacketSend.RequestGameRestart();
                    reloadRequestSent = true;
                }
            }
        }
        else
        {
            gameTimer += Time.deltaTime;
            statisticsFrame.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Tiempo: " + Mathf.FloorToInt(gameTimer) + " s";
            statisticsFrame.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Distancia recorrida: " + System.Math.Round(transform.position.z, 2) + " Km";
            statisticsFrame.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Calorías: " + System.Math.Round(transform.position.z / 50, 2) + " Kcal";
            statisticsFrame.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "Colisiones: " + collisions;
            raceRankFrame.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Posición: " + position;
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
}