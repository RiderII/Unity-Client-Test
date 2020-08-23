using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementCollision : MonoBehaviour
{
    public Player2 player;
    private bool isColliding = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isColliding) return;
            Debug.Log("HITT!");
            isColliding = true;
            StartCoroutine(Reset());


            player.audioSourceRubbleCrash.clip = player.rubbleCrash;
            player.audioSourceRubbleCrash.Play();

            player.audioSourcePedalo.volume *= 0.20f;
            player.speed *= 0.80f;
            player.collisions += 1;
            if (player.totalScore != 0)
            {
                player.totalScore -= 5;
            }
        }
    }

    IEnumerator Reset()
    {
        yield return new WaitForSeconds(2);
        isColliding = false;
    }
}
