using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FencesBumperCollision : MonoBehaviour
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

            player.audioSourceScrapeSound.clip = player.scrapeSound;
            player.audioSourceScrapeSound.Play();
        }
    }

    IEnumerator Reset()
    {
        yield return new WaitForSeconds(1);
        isColliding = false;
    }
}
