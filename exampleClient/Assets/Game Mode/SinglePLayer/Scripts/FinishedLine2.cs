using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class FinishedLine2 : MonoBehaviour
{
    public Player2 player;
    private int laps = 1;
    private int lapsDone = 0;
    public GameObject alert;

    private void Awake()
    {
        if (Client.instance.gameModeSelected == "Multiplayer" || SystemInfo.supportsGyroscope)
        {
            gameObject.SetActive(false);
        }

        if (SceneManager.GetActiveScene().name != "4.6 kilómetros") {
            switch (Client.instance.levelSelected)
            {
                case "200 metros": laps = Constants.twoHundredmeterLaps; break;
                case "500 metros": laps = Constants.fiveHundredmeterLaps; break;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (player.steps.Contains(this.name))
            {
                if (player.ptArrow && player.ptArrow.transform.position.x == transform.position.x &&
                    player.ptArrow.transform.position.z == transform.position.z)
                {
                    player.arrowActive = false;
                    this.gameObject.GetComponent<MeshRenderer>().enabled = false;
                    player.audioSourceCheckPoint.clip = player.checkPoint;
                    player.audioSourceCheckPoint.Play();
                    Destroy(player.ptArrow);
                }
                if (int.Parse(player.lastglass) > int.Parse(this.name))
                {
                    player.lastglass = this.name;
                    player.lastGlassRef = gameObject;
                    player.lastPosition = transform.position;
                    alert.SetActive(true);
                }
                else
                {
                    player.lastglass = this.name;
                    player.lastGlassRef = gameObject;
                    player.lastPosition = transform.position;
                    alert.SetActive(false);
                }
            }
            if (!player.steps.Contains(this.name))
            {
                player.lastglass = this.name;
                player.lastGlassRef = gameObject;
                player.lastPosition = transform.position;
                alert.SetActive(false);
            }
            if (!player.steps.Contains(this.name) && (player.steps.Count + 1).ToString() == this.name)
            {
                player.lastPosition = transform.position;
                player.steps.Add(this.name);
            }
            if (player.steps.Count == 86 && SceneManager.GetActiveScene().name == "200 metros")
            {
                if (this.name == "86")
                {
                    lapsDone++;
                    if (SceneManager.GetActiveScene().name != "4.6 kilómetros")
                    {
                        TrainingManager2.lapsFrame.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Vuelta " + (lapsDone + 1).ToString() + " / " + laps.ToString();
                    }
                        
                    if (lapsDone == laps)
                    {
                        player.reachedFinishLine = true;
                    }
                    player.steps.Clear();
                }
            }
            if (player.steps.Count == 146 && SceneManager.GetActiveScene().name == "500 metros")
            {
                if (this.name == "146")
                {
                    lapsDone++;
                    if (SceneManager.GetActiveScene().name != "4.6 kilómetros")
                    {
                        TrainingManager2.lapsFrame.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Vuelta " + (lapsDone + 1).ToString() + " / " + laps.ToString();
                    }
                    if (lapsDone == laps)
                    {
                        player.reachedFinishLine = true;
                    }
                    player.steps.Clear();
                }
            }
        }
    }
}
