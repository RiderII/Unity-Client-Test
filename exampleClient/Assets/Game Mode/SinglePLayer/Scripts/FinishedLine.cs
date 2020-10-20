using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishedLine : MonoBehaviour
{
    public Player player;
    public Player2 player2;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (player) {
                player.reachedFinishLine = true;
            } else {
                player2.reachedFinishLine = true;
            }
        }
    }
}
