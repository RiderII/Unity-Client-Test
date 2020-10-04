using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementCollision : MonoBehaviour
{
    public Player2 player;
    public GameObject FloatingTextPrefab;
    public GameObject pointingArrow;
    private bool isColliding = false;

    public void Awake()
    {
        if ((Client.instance.gameModeSelected == "Multiplayer" || SystemInfo.supportsGyroscope) && tag != "RampUp" && tag != "RampDown")
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (tag == "RampUp" && Client.instance.gameModeSelected != "Multiplayer" && !SystemInfo.supportsGyroscope)
        {
            StartCoroutine(SlowDown());
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (tag == "NotRoad" && !player.arrowActive)
            {
                player.arrowActive = true;
                ShowPointingArrow();
            }
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
                //player.transform.position = player.transform.position + new Vector3(Mathf.Sin(Time.time * 2f), Mathf.Sin(Time.time * 2f), Mathf.Sin(Time.time * 2f));
                player.audioSourceRubbleCrash.clip = player.rubbleCrash;
                player.audioSourceRubbleCrash.Play();
                ShowFloatingText();
                DecreasePoints();
            }
            else if (tag == "Rock")
            {
                player.audioSourceScrapeSound.clip = player.scrapeSound;
                player.audioSourceScrapeSound.volume = 0.5f;
                player.audioSourceScrapeSound.Play();
                ShowFloatingText();
                DecreasePoints();
            }
            else if (tag == "Tree")
            {
                player.audioSourceHitTree.clip = player.hitTree;
                player.audioSourceHitTree.Play();       
            }

            else if (tag == "RampUp" && Client.instance.gameModeSelected != "Multiplayer" && !SystemInfo.supportsGyroscope)
            {
                player.speed += 10;
                player.surpassSpeed = true;
                player.audioSourcePedalo.Stop();
                player.audioSourcePedaleoFaster.clip = player.pedaleoFaster;
                player.audioSourcePedaleoFaster.volume = 1.5f;
                player.audioSourcePedaleoFaster.Play();
            }

            if (tag != "RampUp" && tag != "RampDown" && tag != "NotRoad")
            {
                player.audioSourcePedalo.volume *= 0.20f;
                player.speed *= 0.80f;
                player.collisions += 1;
                if (player.points != 0)
                {
                    player.points -= 5;
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
        GameObject puntaje = Instantiate(FloatingTextPrefab, new Vector3(player.transform.position.x,
            player.transform.position.y, player.transform.position.z), 
            player.transform.rotation, player.transform);
    }

    void ShowPointingArrow()
    {
        player.ptArrow = Instantiate(pointingArrow, new Vector3(player.lastPosition.x,
        -4f, player.lastPosition.z),
        Quaternion.identity);

        player.lastGlassRef.GetComponent<MeshRenderer>().enabled = true;
    }


    void DecreasePoints()
    {
        if (player.points - 100 > 0)
        {
            player.points -= 100;
        }
        else
        {
            player.points = 0;
        }
    }
}
