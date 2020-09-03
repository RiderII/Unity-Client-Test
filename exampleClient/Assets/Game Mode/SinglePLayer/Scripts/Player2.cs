using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2 : MonoBehaviour
{
    public Rigidbody player;

    public int id;
    public int dbId;
    public string username = "diego";
    public string email = "test@test.com";
    public int collisions = 0;
    public float traveled_meters = 0f;
    public float burned_calories = 0f;
    public float totalGameTime = 0f;
    public float weight = 90f;
    public int nextUpdate = 1;
    public float playerSpeed = 0f;
    public List<Medal> medals = new List<Medal>();
    public List<MapReport> mapReport;
    public float totalScore = 0;
    public string league = "amateur";
    public List<string> steps = new List<string>();
    public float framesPerSecond = 0f;
    Vector3 oldPos;
    Vector3 oldPos2;

    private float yVelocity = 0;
    public float gravity = -9.81f;
    public float acceleration = 0.6f;
    public float maximunSpeed = 10f;
    public float obstacleSlowDown = 0.25f;
    public float speed = 10f;
    public bool reachedFinishLine = false;
    public bool playPedaleo = false;
    public AudioClip vaquitamu;
    public AudioClip pedaleo;
    public AudioClip bikeBrake;
    public AudioClip rubbleCrash;
    public AudioClip scrapeSound;
    public AudioClip hitTree;
    public AudioSource audioBikeBrake;
    public AudioSource audioSourceVaquita;
    public AudioSource audioSourcePedalo;
    public AudioSource audioSourceRubbleCrash;
    public AudioSource audioSourceScrapeSound;
    public AudioSource audioSourceHitTree;
    //public AudioSource pedaleo;

    private void Awake()
    {
        if (Client.instance.gameModeSelected == "Multiplayer")
        {
            gameObject.SetActive(false);
        }
    }
    // Start is called before the first frame update
    [SerializeField] private GameObject pausePanel;
    void Start()
    {
        Time.timeScale = 1;
        oldPos = transform.position;
        oldPos2 = transform.position;
        pausePanel.SetActive(false);
        audioSourceVaquita = AddAudio(false, false, 1.0f);
        audioSourcePedalo = AddAudio(true, false, 0.1f);
        audioBikeBrake = AddAudio(false, false, 1.0f);
        audioSourceRubbleCrash = AddAudio(false, false, 1.0f);
        audioSourceScrapeSound = AddAudio(false, false, 1.0f);
        audioSourceHitTree = AddAudio(false, false, 1.0f);
    }

    public AudioSource AddAudio(bool loop, bool playAwake, float vol)
    {
        AudioSource newAudio = gameObject.AddComponent<AudioSource>();
        newAudio.loop = loop;
        newAudio.playOnAwake = playAwake;
        newAudio.volume = vol;
        return newAudio;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Time.time >= nextUpdate)
        {
            nextUpdate = Mathf.FloorToInt(Time.time) + 1;
            Vector3 distanceVector = (transform.position - oldPos2);
            float distanceThisFrame = distanceVector.magnitude;
            oldPos2 = transform.position;

            playerSpeed = distanceThisFrame; // mph
        }

        //pause event
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (!pausePanel.activeInHierarchy)
            {
                PauseGame();
            }
            else
            {
                ContinueGame();
            }
        }

        // Acceleration logic

        speed += acceleration * Time.deltaTime;

        // Control pedaling volume if user is pedaling or not.

        if (playPedaleo)
        {
            audioSourcePedalo.volume *= 1.018f;
        }
        else
        {
            audioSourcePedalo.volume /= 1.02f;
        }


        if (speed > maximunSpeed)
        {
            speed = maximunSpeed;
        }

        if (speed < 0)
        {
            //detener pedaleo
            speed = 0;
        }
        else
        {
            Vector3 distanceVector = (transform.position - oldPos);
            float distanceThisFrame = distanceVector.magnitude;
            traveled_meters += distanceThisFrame;
            oldPos = transform.position;
            burned_calories += Utils.CaloriesBurned(weight, (playerSpeed * 60) *60);
        }

        if (acceleration < 0)
        {
            player.transform.Translate(speed * Time.deltaTime, 0f, 0f);
        }

        // Make player move automatically

        // Vector3 direction = new Vector3(transform.parent.forward.x, 0, transform.parent.forward.z);
        // transform.parent.position += direction.normalized * speed * Time.deltaTime;
        if (Input.GetKey("w") && !reachedFinishLine)
        {
            if (!playPedaleo)
            {
                audioSourcePedalo.clip = pedaleo;
                audioSourcePedalo.Play();
                playPedaleo = true;
            }

            if (acceleration < 0)
            {

                acceleration *= -1;
                //reproducir pedaleo
            }
            player.transform.Translate(speed * Time.deltaTime, 0f, 0f);
        }

        if (Input.GetKeyUp("w") && !reachedFinishLine)
        {
            acceleration *= -1;
            playPedaleo = false;
        }

        if (reachedFinishLine && acceleration > 0)
        {
            if (speed > 7)
            {
                acceleration *= -5;
            }
            else if (speed > 5)
            {
                acceleration *= -3;
            }
            else
            {
                acceleration *= -1;

            }
            
            playPedaleo = false;
            audioBikeBrake.clip = bikeBrake;
            audioBikeBrake.volume = speed;
            audioBikeBrake.Play();
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0;
        pausePanel.SetActive(true);
        //Disable scripts that still work while timescale is set to 0
    }
    private void ContinueGame()
    {
        Time.timeScale = 1;
        pausePanel.SetActive(false);
        //enable the scripts again
    }
}
