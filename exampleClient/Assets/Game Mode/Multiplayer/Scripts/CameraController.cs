using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public PlayerManager player;
    private GyroManager gyroInstance;

    public float sensitivity = 100f;
    public float clamAngle = 85f;
    public float verticalRotation;
    public float horizontalRotation;

    public float defaultVolumeCollision = 1.0f;

    public bool playPedaleo = false;
    public static bool playVaquitaMu = false;
    public static bool playRubbleCrash = false;
    public static bool playPedaleoFaster = false;
    public static Vector3 collisionPosition;
    public AudioClip vaquitamu;
    public AudioClip pedaleo;
    public AudioClip rubbleCrash;
    public AudioClip pedaleoFaster;
    public static AudioSource audioSourceVaquita;
    public static AudioSource audioSourcePedalo;
    public static AudioSource audioSourceRubbleCrash;
    public static AudioSource audioSourcePedaleoFaster;

    private void Start()
    {
        verticalRotation = transform.localEulerAngles.x;
        horizontalRotation = player.transform.eulerAngles.y;
        gyroInstance = GyroManager.Instance;
        gyroInstance.EnableGyro();
        audioSourceVaquita = AddAudio(false, false, defaultVolumeCollision);
        audioSourcePedalo = AddAudio(true, false, 0.5f);
        audioSourceRubbleCrash = AddAudio(false, false, defaultVolumeCollision);
        audioSourcePedaleoFaster = AddAudio(true, false, 1.0f);
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

        Look();

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

        if (playPedaleoFaster)
        {
            playPedaleoFaster = false;
            float soundVolume = Mathf.Sqrt(Mathf.Pow((Mathf.Round(player.transform.position.x) - Mathf.Round(collisionPosition.x)), 2) + Mathf.Pow(Mathf.Round((player.transform.position.z) - Mathf.Round(collisionPosition.z)), 2));
            soundVolume = soundVolume / 20f;
            //if (soundVolume != 0) defaultVolumeCollision = 0.5f;
            Debug.Log($"Volume {soundVolume}");

            audioSourcePedaleoFaster.volume = defaultVolumeCollision - soundVolume;
            audioSourcePedaleoFaster.clip = pedaleoFaster;
            audioSourcePedaleoFaster.Play();
        }

        if (playRubbleCrash)
        {
            playRubbleCrash = false;
            float soundVolume = Mathf.Sqrt(Mathf.Pow((Mathf.Round(player.transform.position.x) - Mathf.Round(collisionPosition.x)), 2) + Mathf.Pow(Mathf.Round((player.transform.position.z) - Mathf.Round(collisionPosition.z)), 2));
            soundVolume = soundVolume / 20f;
            //if (soundVolume != 0) defaultVolumeCollision = 0.5f;
            Debug.Log($"Volume {soundVolume}");
            audioSourceRubbleCrash.volume = defaultVolumeCollision - soundVolume;
            audioSourceRubbleCrash.clip = rubbleCrash;
            audioSourceRubbleCrash.Play();
        }
    }

    private void Look()
    {
        if (GameManager.players.ContainsKey(Client.instance.myId) && !GameManager.players[Client.instance.myId].finishedGame)
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

                float rightSpan = 0f;

                if (GameManager.instance.sceneName != "Vaquita")
                {
                    rightSpan = 90f;
                }

                transform.localRotation = Quaternion.Euler(verticalRotation, rightSpan, 0f);
                player.transform.rotation = Quaternion.Euler(0f, horizontalRotation, 0f);
            }
        }
    }
}
