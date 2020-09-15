
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TrainingManager2 : MonoBehaviour
{
    public Player2 player;
    //public TextMeshProUGUI textTimer;

    // for ingame canvas
    public Image UIpanel;
    public Canvas playerCanvas;
    private GameObject playersFrame;
    private GameObject statisticsFrame;
    private GameObject displayInfoFrame;
    private GameObject raceRankFrame;
    public GameObject finishLine;
    private GyroManager gyroInstance;
    private int previouSteps = 0;

    // for race results
    private GameObject playersFrameResult;

    public float spawnDistanceFromPlayer = 40f;
    public float spawnDistanceFromObstacles = 10f;
    public int numberOfObstaclers = 0;

    private float obstaclePointer;

    private float gameTimer;
    private float distanceTimer;
    private float finalTime;
    private bool isGameOver = false;
    private string sceneName;

    public float gameOverTimer = 3f;

    [SerializeField] private GameObject raceResults;
    // Start is called before the first frame update

    private void Awake()
    {
        if (Client.instance.gameModeSelected == "Multiplayer")
        {
            gameObject.SetActive(false);
        }
    }
    void Start()
    {
        gyroInstance = GyroManager.Instance;
        gyroInstance.EnableGyro();
        if (gyroInstance.GetGyroActive())
        {
            Client.instance.userName = DataBridge.instance.userProfile.username;
            Client.instance.ConnectToServer("18.191.13.53");
        }
        Time.timeScale = 1;
        Image uiPanel = Instantiate(UIpanel, playerCanvas.transform);
        playersFrame = uiPanel.transform.GetChild(0).gameObject;
        statisticsFrame = uiPanel.transform.GetChild(1).gameObject;
        displayInfoFrame = uiPanel.transform.GetChild(2).gameObject;
        raceRankFrame = uiPanel.transform.GetChild(3).gameObject;
        displayInfoFrame.SetActive(false);
        raceRankFrame.SetActive(false);
        playersFrame.SetActive(false);
        raceResults.SetActive(false);

        sceneName = SceneManager.GetActiveScene().name;
    }

    // Update is called once per frame
    void Update()
    {
        gameTimer += Time.deltaTime;
        distanceTimer += Time.deltaTime;

        if (isGameOver == false)
        {
            if (player.reachedFinishLine == true)
            {
                CalculateScore();
                isGameOver = true;
                finalTime = gameTimer;
                player.totalGameTime = finalTime;
                Challenge challengeType = new Challenge("reto de 200 km", 20);
                Medal medal = new Medal(challengeType, "sprite", System.DateTime.Now, "lo lograste");
                player.medals.Add(medal);
                MapReport mapReport = new MapReport(
                    player.collisions, player.traveled_meters, player.burned_calories,
                    player.totalGameTime, System.DateTime.Now.ToString(), medal);
                DataBridge.instance.SaveReport(mapReport);
                //verificar si gano una medalla
                CheckRecords(mapReport);
            }

            if (distanceTimer >= 5f)
            {
                int stepsPassed = player.steps.Count - previouSteps;
                CalculatePoints(stepsPassed);
                previouSteps = player.steps.Count;
                distanceTimer = 0f;
            }

            statisticsFrame.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Puntos: " + player.points;
            statisticsFrame.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Tiempo: " + Mathf.FloorToInt(gameTimer) + " s";
            statisticsFrame.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Velocidad: " + Mathf.FloorToInt((player.playerSpeed * 60 * 60 )/ 1000) + " kmph";
            statisticsFrame.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "Distancia recorrida: " + System.Math.Round(player.traveled_meters, 2) + " m";
            statisticsFrame.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = "Calorías: " + System.Math.Round(player.burned_calories, 2) + " Kcal";
            statisticsFrame.transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = "Colisiones: " + player.collisions;
        }
        else
        {
            displayInfoFrame.SetActive(true);
            displayInfoFrame.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Game over! \nTú tiempo: " + Mathf.FloorToInt(finalTime);
            gameOverTimer -= Time.deltaTime;

            if (gameOverTimer <= 0f)
            {
                ShowFinishDashboard();
                playerCanvas.enabled = false;
                //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

        }
    }

    void CheckRecords(MapReport mapReport)
    {
        if (DataBridge.instance.GetMode() == "Fun")
        {
            if (mapReport.totalGameTime < 30)
            {
                DataBridge.instance.SaveUserMedal("one");
            }
            if (mapReport.collisions == 0)
            {
                DataBridge.instance.SaveUserMedal("two");
            }
            if (mapReport.totalGameTime <= 24)
            {
                DataBridge.instance.SaveUserMedal("three");
            }
        }
        if (DataBridge.instance.GetMode() == "Fitness")
        {
            if (mapReport.totalGameTime < 27)
            {
                DataBridge.instance.SaveUserMedal("one");
            }
            if (mapReport.totalGameTime <= 24)
            {
                DataBridge.instance.SaveUserMedal("two");
            }
            if (mapReport.totalGameTime <= 21)
            {
                DataBridge.instance.SaveUserMedal("three");
            }
        }
    }

    private void CalculatePoints(int stepsPassed)
    {
        switch (stepsPassed)
        {
            case var _ when stepsPassed >= 12: player.points += 250; break;
            case var _ when stepsPassed >= 10: player.points += 200; break;
            case var _ when stepsPassed >= 8: player.points += 150; break;
            case var _ when stepsPassed >= 6: player.points += 100; break;
        }
    }

    private void CalculateScore()
    {
        //double distance = System.Math.Round(player.traveled_meters, 2);
        //double calories = System.Math.Round(player.burned_calories, 2);

        //switch (distance)
        //{
        //    case var _ when distance > 100: player.totalScore += 50; break;
        //    case var _ when distance > 200: player.totalScore += 100; break;
        //    case var _ when distance > 300: player.totalScore += 200; break;
        //}

        //switch (calories)
        //{
        //    case var _ when calories > 2: player.totalScore += 30; break;
        //    case var _ when calories > 4: player.totalScore += 60; break;
        //    case var _ when calories > 10: player.totalScore += 90; break;
        //}

        playersFrameResult = raceResults.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject;
        playersFrameResult.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = player.username;
        playersFrameResult.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "TIEMPO: " + Mathf.FloorToInt(gameTimer) + " s";
        playersFrameResult.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = "DISTANCIA RECORRIDA: " + System.Math.Round(player.traveled_meters, 2) + " m";
        playersFrameResult.transform.GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>().text = "CALORIAS: " + System.Math.Round(player.burned_calories, 2) + " Kcal";
        playersFrameResult.transform.GetChild(1).GetChild(3).GetComponent<TextMeshProUGUI>().text = "COLISIONES: " + player.collisions;
        playersFrameResult.transform.GetChild(1).GetChild(4).GetComponent<TextMeshProUGUI>().text = "PUNTAJE: " + player.points;
        playersFrameResult.transform.GetChild(1).GetChild(5).GetComponent<TextMeshProUGUI>().text = "HORA DE INICIO " + System.DateTime.Now;
    }

    private void ShowFinishDashboard()
    {
        Time.timeScale = 1;
        raceResults.SetActive(true);
        //Disable scripts that still work while timescale is set to 0
    }
}
