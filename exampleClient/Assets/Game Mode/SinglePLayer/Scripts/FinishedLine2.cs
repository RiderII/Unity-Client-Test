using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishedLine2 : MonoBehaviour
{
    public Player2 player;

    private void Awake()
    {
        if (Client.instance.gameModeSelected == "Multiplayer")
        {
            gameObject.SetActive(false);
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
                player.reachedFinishLine = true;
            }
            if (player.steps.Count == 146 && SceneManager.GetActiveScene().name == "500 metros")
            {
                player.reachedFinishLine = true;
            }
        }
    }
}
