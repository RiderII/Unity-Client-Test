using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public int id;
    public string username;
    public int collisions;
    public float traveled_kilometers;
    public float burned_calories;
    public bool finishedGame = false;
    public bool reloadRequestSent = false;
    public TextMesh infoText;

    private float gameOverTimer = 3f;

    private float gameTimer;
    private float finalTime;

    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;
    }

    public void SetCollisions(int _collision)
    {
        collisions = _collision;
    }

    private void FixedUpdate()
    {
        if (finishedGame)
        {
            finalTime = gameTimer;
            infoText.text = "Ruta finalizada\n Tu tiempo:" + Mathf.FloorToInt(finalTime);
            gameOverTimer -= Time.deltaTime;
            if (gameOverTimer <= 0f)
            {
                if (!reloadRequestSent) {
                    ClientSend.RequestGameRestart();
                    reloadRequestSent = true;
                }
            }
        }
        else {
            gameTimer += Time.deltaTime;
            infoText.text = "Tiempo: " + Mathf.FloorToInt(gameTimer);
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
    }
}
