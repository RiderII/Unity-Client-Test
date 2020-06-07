using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public PlayerManager player;
    public float sensitivity = 100f;
    public float clamAngle = 85f;

    private float verticalRotation;
    private float horizontalRotation;

    public bool playPedaleo = false;
    public AudioClip vaquitamu;
    public AudioClip pedaleo;
    public AudioSource audioSourceVaquita;
    public AudioSource audioSourcePedalo;

    private void Start()
    {
        verticalRotation = transform.localEulerAngles.x;
        horizontalRotation = player.transform.eulerAngles.y;
        audioSourceVaquita = AddAudio(false, false, 1.0f);
        audioSourcePedalo = AddAudio(true, false, 0.5f);
    }

    public AudioSource AddAudio(bool loop, bool playAwake, float vol)
    {
        AudioSource newAudio = gameObject.AddComponent<AudioSource>();
        newAudio.loop = loop;
        newAudio.playOnAwake = playAwake;
        newAudio.volume = vol;
        return newAudio;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) //key to unlock and lock the mouse
        {
            ToggleCursorMode();
        }
        if (Cursor.lockState == CursorLockMode.None)
        {
            Look();
        }
        Debug.DrawRay(transform.position, transform.forward * 2, Color.red);

        if (playPedaleo)
        {
            audioSourcePedalo.volume = 0.5f;
        }
        else
        {
            audioSourcePedalo.volume = 0f;
        }

        if (Input.GetKey(KeyCode.W))
        {

            if (!playPedaleo)
            {
                audioSourcePedalo.clip = pedaleo;
                audioSourcePedalo.Play();
                playPedaleo = true;
            }
        }

        if (Input.GetKeyUp(KeyCode.W))
        {
            playPedaleo = false;
        }
    }

    private void Look()
    {
        float _mouseVertical = -Input.GetAxis("Mouse Y");
        float _mouseHorizontal = Input.GetAxis("Mouse X");

        verticalRotation += _mouseVertical * sensitivity * Time.deltaTime;
        horizontalRotation += _mouseHorizontal * sensitivity * Time.deltaTime;

        verticalRotation = Mathf.Clamp(verticalRotation, -clamAngle, clamAngle);

        transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        player.transform.rotation = Quaternion.Euler(0f, horizontalRotation, 0f);
    }

    public void ToggleCursorMode()
    {
        Cursor.visible = !Cursor.visible;

        if (Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
