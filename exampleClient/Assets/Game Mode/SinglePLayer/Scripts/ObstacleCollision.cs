using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleCollision : MonoBehaviour
{
    private bool isColliding = false;

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.transform.tag == "Obstacle")
        {
            if (isColliding) return;
            Debug.Log("HITT!");
            isColliding = true;
            StartCoroutine(Reset());

            Player player = hit.controller.gameObject.GetComponentInChildren<Player>();
            player.audioSourceVaquita.clip = player.vaquitamu;
            player.audioSourceVaquita.Play();
            player.audioSourcePedalo.volume *= 0.20f;
            player.speed *= player.obstacleSlowDown;
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
