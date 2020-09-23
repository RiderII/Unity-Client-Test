using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishedLine2 : MonoBehaviour
{
    public Player2 player;
    private int laps = 1;
    private int lapsDone = 0;

    private void Awake()
    {
        if (Client.instance.gameModeSelected == "Multiplayer")
        {
            gameObject.SetActive(false);
        }

        switch (Client.instance.levelSelected)
        {
            case "200 metros": laps = Constants.twoHundredmeterLaps; break;
            case "500 metros": laps = Constants.fiveHundredmeterLaps; break;
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
            if (!player.steps.Contains(this.name) && (player.steps.Count + 1).ToString() == this.name)
            {
                player.steps.Add(this.name);
            }
            if (player.steps.Count == 86 && SceneManager.GetActiveScene().name == "200 metros")
            {
                if (this.name == "86")
                {
                    lapsDone++;
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
