using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishedLine : MonoBehaviour
{
    public Player player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player.reachedFinishLine = true;
        }
    }
}
