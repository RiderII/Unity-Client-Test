using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrainingManager : MonoBehaviour
{
    public GameObject obstaclePrefab;
    public Player player;
    public TextMesh infoText;
    public GameObject finishLine;

    public float spawnDistanceFromPlayer = 40f;
    public float spawnDistanceFromObstacles = 10f;
    public float finishLinePosition = 200f;

    private float obstaclePointer;

    private float gameTimer;
    private float finalTime;
    private bool isGameOver = false;

    public float gameOverTimer = 3f;

    // Start is called before the first frame update
    void Start()
    {
        finishLine.transform.position = new Vector3(0, 0, finishLinePosition);
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
        }

        gameTimer += Time.deltaTime;

        if (isGameOver == false)
        {
            if (player.reachedFinishLine == true)
            {
                isGameOver = true;
                finalTime = gameTimer;
            }

            infoText.text = "Time: " + Mathf.FloorToInt(gameTimer);
        }
        else
        {
            infoText.text = "Game over! \nYour time: " + Mathf.FloorToInt(finalTime);
            gameOverTimer -= Time.deltaTime;

            if (gameOverTimer <= 0f)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

        }
    }
}
