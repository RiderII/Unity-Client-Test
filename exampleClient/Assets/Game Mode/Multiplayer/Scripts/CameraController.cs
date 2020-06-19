using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public PlayerManager player;
    private GyroManager gyroInstance;

    public float sensitivity = 100f;
    public float clamAngle = 85f;
    private float verticalRotation;
    private float horizontalRotation;

    public float defaultVolumeCollision = 1.0f;

    public bool playPedaleo = false;
    public static bool playVaquitaMu = false;
    public static Vector3 collisionPosition;
    public AudioClip vaquitamu;
    public AudioClip pedaleo;
    public static AudioSource audioSourceVaquita;
    public static AudioSource audioSourcePedalo;

    private void Start()
    {
        verticalRotation = transform.localEulerAngles.x;
        horizontalRotation = player.transform.eulerAngles.y;
        gyroInstance = GyroManager.Instance;
        gyroInstance.EnableGyro();
        audioSourceVaquita = AddAudio(false, false, defaultVolumeCollision);
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
            audioSourcePedalo.volume *= 1.018f;
        }
        else
        {
            audioSourcePedalo.volume /= 1.02f;
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

        if (playVaquitaMu)
        {
            playVaquitaMu = false;
            float soundVolume = Mathf.Sqrt(Mathf.Pow((Mathf.Round(player.transform.position.x) - Mathf.Round(collisionPosition.x)), 2) + Mathf.Pow(Mathf.Round((player.transform.position.z) - Mathf.Round(collisionPosition.z)), 2));
            soundVolume = soundVolume / 20f;
            //if (soundVolume != 0) defaultVolumeCollision = 0.5f;
            Debug.Log($"Volume {soundVolume}");
            audioSourceVaquita.volume = defaultVolumeCollision - soundVolume;
            audioSourceVaquita.clip = vaquitamu;
            audioSourceVaquita.Play();
        }
    }

    private void Look()
    {
        // centrar el celular para obtener el posicionamiento deseado
        if (gyroInstance.GetGyroActive())
        {
            player.transform.rotation = Quaternion.Euler(0f, (gyroInstance.GetGyroRotation().y + 0.3f) * 60.5f, 0f);
            transform.localRotation = Quaternion.Euler((gyroInstance.GetGyroRotation().x + 0.5f) * 60.5f, 0f, 0f);
        }
        else
        {
            float _mouseVertical = -Input.GetAxis("Mouse Y");
            float _mouseHorizontal = Input.GetAxis("Mouse X");

            verticalRotation += _mouseVertical * sensitivity * Time.deltaTime;
            horizontalRotation += _mouseHorizontal * sensitivity * Time.deltaTime;

            verticalRotation = Mathf.Clamp(verticalRotation, -clamAngle, clamAngle);

            transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
            player.transform.rotation = Quaternion.Euler(0f, horizontalRotation, 0f);
        }
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
