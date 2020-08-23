using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishedLine2 : MonoBehaviour
{
    public Player2 player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Collided");
            player.reachedFinishLine = true;
            player.traveled_kilometers = player.transform.position.z;
        }
    }
}
