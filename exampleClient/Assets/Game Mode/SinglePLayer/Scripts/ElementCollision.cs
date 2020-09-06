using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementCollision : MonoBehaviour
{
    public Player2 player;
    private bool isColliding = false;

    public void Awake()
    {
        if (Client.instance.gameModeSelected == "Multiplayer")
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isColliding) return;
            Debug.Log("HITT!");
            isColliding = true;
            StartCoroutine(Reset());

            if (tag == "Tires")
            {
                player.audioSourceRubbleCrash.clip = player.rubbleCrash;
                player.audioSourceRubbleCrash.Play();
            }
            else if (tag == "Rock")
            {
                player.audioSourceScrapeSound.clip = player.scrapeSound;
                player.audioSourceScrapeSound.volume = 0.5f;
                player.audioSourceScrapeSound.Play();
            }
            else if (tag == "Tree")
            {
                player.audioSourceHitTree.clip = player.hitTree;
                player.audioSourceHitTree.Play();
            }

            else if (tag == "Ramp")
            {
                player.speed += 5;
                player.surpassSpeed = true;
            }


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
        yield return new WaitForSeconds(3);
        isColliding = false;
        player.surpassSpeed = false;
        while (player.maximunSpeed <= player.speed)
        {
            player.speed -= 2;
        }
    }
}
