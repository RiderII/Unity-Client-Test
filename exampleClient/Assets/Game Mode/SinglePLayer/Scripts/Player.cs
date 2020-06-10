using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float mouseSensitivity = 100.0f;
    public float clampAngle = 80.0f;
    private float rotY = 0.0f; // rotation around the up/y axis

    public CharacterController controller;

    private float yVelocity = 0;
    public float gravity = -9.81f;
    public float acceleration = 0.6f;
    public float maximunSpeed = 10f;
    public float obstacleSlowDown = 0.25f;
    public float speed = 0f;
    public bool reachedFinishLine = false;
    public bool playPedaleo = false;
    public AudioClip vaquitamu;
    public AudioClip pedaleo;
    public AudioSource audioSourceVaquita;
    public AudioSource audioSourcePedalo;
    //public AudioSource pedaleo;

    // Start is called before the first frame update
    [SerializeField] private GameObject pausePanel;
    void Start()
    {
        Vector3 rot = transform.localRotation.eulerAngles;
        rotY = rot.y;

        pausePanel.SetActive(false);
        audioSourceVaquita = AddAudio(false, false, 1.0f);
        audioSourcePedalo = AddAudio(true, false, 0.1f);
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
    void Update()
    {
        float _mouseVertical = -Input.GetAxis("Mouse Y");
        rotY += _mouseVertical * mouseSensitivity * Time.deltaTime;
        rotY = Mathf.Clamp(rotY, -clampAngle, clampAngle);
        transform.localRotation = Quaternion.Euler(rotY, 0f, 0f);

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

        if (controller.isGrounded)
        {
            yVelocity = 0f;
        }
        else {
            Vector3 moveDown = new Vector3(controller.transform.forward.x, controller.transform.forward.y + gravity, controller.transform.forward.z);
            controller.Move(moveDown.normalized * speed * Time.deltaTime);
        }
        yVelocity += gravity;

        Vector3 direction = new Vector3(controller.transform.forward.x, 0, controller.transform.forward.z);

        speed += acceleration * Time.deltaTime;

        // Control pedaling volume if user is pedaling or not.

        if (playPedaleo)
        {
            audioSourcePedalo.volume *= 1.018f;
        }
        else {
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

        if (acceleration < 0)
        {
            controller.Move(direction.normalized * speed * Time.deltaTime);
        }

        // Make player move automatically

        // Vector3 direction = new Vector3(transform.parent.forward.x, 0, transform.parent.forward.z);
        // transform.parent.position += direction.normalized * speed * Time.deltaTime;

        if (Input.GetKey("up")) {

            if (!playPedaleo) {
                audioSourcePedalo.clip = pedaleo;
                audioSourcePedalo.Play();
                playPedaleo = true;
            }
            
            if (acceleration < 0) {
                
                acceleration *= -1;
                //reproducir pedaleo
            }
            controller.Move(direction.normalized * speed * Time.deltaTime);
        }

        if (Input.GetKeyUp("up"))
        {
            acceleration *= -1;
            playPedaleo = false;
        }




        // Make player stay inside a certain area
        if (controller.transform.position.x < -4.5f)
        {
            controller.enabled = false;
            controller.transform.position = new Vector3(-4.5f, controller.transform.position.y, controller.transform.position.z);
            controller.enabled = true;
        }
        else if (controller.transform.position.x > 4.5f) {
            controller.enabled = false;
            controller.transform.position = new Vector3(4.5f, controller.transform.position.y, controller.transform.position.z);
            controller.enabled = true;
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
