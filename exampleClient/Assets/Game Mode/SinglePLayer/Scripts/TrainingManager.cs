
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TrainingManager : MonoBehaviour
{
    public GameObject obstaclePrefab;
    public Player player;
    //public TextMeshProUGUI textTimer;
    public Image UIpanel;
    private GameObject playersFrame;
    private GameObject statisticsFrame;
    private GameObject displayInfoFrame;
    private GameObject raceRankFrame;
    public GameObject finishLine;
   
    public float spawnDistanceFromPlayer = 40f;
    public float spawnDistanceFromObstacles = 10f;
    public float finishLinePosition = 200f;
    public int numberOfObstaclers = 0;

    private float obstaclePointer;

    private float gameTimer;
    private float finalTime;
    private bool isGameOver = false;

    public float gameOverTimer = 3f;

    // Start is called before the first frame update
    void Start()
    {
        finishLine.transform.position = new Vector3(0, 0, finishLinePosition);
        playersFrame = UIpanel.transform.GetChild(0).gameObject;
        statisticsFrame = UIpanel.transform.GetChild(1).gameObject;
        displayInfoFrame = UIpanel.transform.GetChild(2).gameObject;
        raceRankFrame = UIpanel.transform.GetChild(3).gameObject;
        displayInfoFrame.SetActive(false);
        //raceRankFrame.SetActive(false);
        //playersFrame.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (obstaclePointer < player.transform.parent.position.z)
        {
            obstaclePointer += spawnDistanceFromObstacles;

            GameObject obstacleObject = Instantiate(obstaclePrefab);
            obstacleObject.transform.position = new Vector3(
                Random.Range(-4f, 4f),
                1.5f,
                player.transform.parent.position.z + spawnDistanceFromPlayer
            );
            numberOfObstaclers += 1;
        }

        gameTimer += Time.deltaTime;

        if (isGameOver == false)
        {
            if (player.reachedFinishLine == true)
            {
                isGameOver = true;
                finalTime = gameTimer;
                player.totalGameTime = finalTime;
                Challenge challengeType = new Challenge("reto de 200 km", 20);
                Medal medal = new Medal(challengeType, "sprite", System.DateTime.Now, "lo lograste");
                player.medals.Add(medal);
                MapReport mapReport = new MapReport(
                    player.collisions, player.traveled_kilometers, player.burned_calories,
                    player.totalGameTime, System.DateTime.Now.ToString(), medal);
                DataBridge.instance.SaveReport(mapReport);
                //verificar si gano una medalla
                CheckRecords(mapReport);    
            }

            statisticsFrame.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Tiempo: " + Mathf.FloorToInt(gameTimer);
            statisticsFrame.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Distancia recorrida: " + System.Math.Round(player.controller.transform.position.z, 2);
            statisticsFrame.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Calorías: " + System.Math.Round(player.controller.transform.position.z / 50, 2);
            statisticsFrame.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "Colisiones: " + player.collisions;
        }
        else
        {
            displayInfoFrame.SetActive(true);
            displayInfoFrame.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Game over! \nTú tiempo: " + Mathf.FloorToInt(finalTime);
            gameOverTimer -= Time.deltaTime;

            if (gameOverTimer <= 0f)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

        }
    }

    void CheckRecords(MapReport mapReport)
    {
        if(DataBridge.instance.GetMode() == "Fun")
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
}
