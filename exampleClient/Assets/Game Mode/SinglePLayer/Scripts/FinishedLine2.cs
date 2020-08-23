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
            if (!player.steps.Contains(tag))
            {
                player.steps.Add(tag);
            }
            if (player.steps.Count == 4 && tag == "FinishLine")
            {
                player.reachedFinishLine = true;
                player.traveled_kilometers = player.transform.position.z;
            }
        }
    }
}
