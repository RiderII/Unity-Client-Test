using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementCollision : MonoBehaviour
{
    public Player2 player;
    public GameObject FloatingTextPrefab;
    private bool isColliding = false;

    public void Awake()
    {
        if (Client.instance.gameModeSelected == "Multiplayer" && tag != "RampUp" && tag != "RampDown")
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (tag == "RampUp")
        {
            StartCoroutine(SlowDown());
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
                ShowFloatingText();
            }
            else if (tag == "Rock")
            {
                player.audioSourceScrapeSound.clip = player.scrapeSound;
                player.audioSourceScrapeSound.volume = 0.5f;
                player.audioSourceScrapeSound.Play();
                ShowFloatingText();
            }
            else if (tag == "Tree")
            {
                player.audioSourceHitTree.clip = player.hitTree;
                player.audioSourceHitTree.Play();           
            }

            else if (tag == "RampUp")
            {
                player.speed += 10;
                player.surpassSpeed = true;
                player.audioSourcePedalo.Stop();
                player.audioSourcePedaleoFaster.clip = player.pedaleoFaster;
                player.audioSourcePedaleoFaster.volume = 1.5f;
                player.audioSourcePedaleoFaster.Play();
            }

            if (tag != "RampUp" && tag != "RampDown")
            {
                player.audioSourcePedalo.volume *= 0.20f;
                player.speed *= 0.80f;
                player.collisions += 1;
                if (player.totalScore != 0)
                {
                    player.totalScore -= 5;
                }
            }

        }
    }

    IEnumerator Reset()
    {
        yield return new WaitForSeconds(3);
        isColliding = false;
    }

    IEnumerator SlowDown()
    {
        yield return new WaitForSeconds(2);
        player.surpassSpeed = false;
    }

    void ShowFloatingText()
    {
        Instantiate(FloatingTextPrefab, new Vector3(transform.position.x,
            transform.position.y + 3.5f, transform.position.z), Quaternion.identity, transform);
    }
}
