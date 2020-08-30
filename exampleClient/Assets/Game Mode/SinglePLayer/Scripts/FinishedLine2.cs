using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            if (!player.steps.Contains(tag))
            {
                player.steps.Add(tag);
            }
            if (player.steps.Count == 4 && tag == "FinishLine")
            {
                player.reachedFinishLine = true;
            }
        }
    }
}
