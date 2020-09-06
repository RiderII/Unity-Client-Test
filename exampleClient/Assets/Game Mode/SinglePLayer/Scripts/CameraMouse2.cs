﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMouse2 : MonoBehaviour
{
    public Player2 player;
    private GyroManager gyroInstance;
    public float sensitivity = 100f;
    public float clamAngle = 45f;
    private float verticalRotation;
    private float horizontalRotation;

    private void Awake()
    {
        if (Client.instance.gameModeSelected == "Multiplayer")
        {
            gameObject.SetActive(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        verticalRotation = transform.eulerAngles.x;
        horizontalRotation = player.transform.localEulerAngles.y;
        gyroInstance = GyroManager.Instance;
        gyroInstance.EnableGyro();
    }

    // Update is called once per frame
    void Update()
    {
        if (!player.reachedFinishLine)
        {
            // centrar el celular para obtener el posicionamiento deseado
            if (gyroInstance.GetGyroActive())
            {
                player.transform.localRotation = Quaternion.Euler((gyroInstance.GetGyroRotation().x + 0.5f) * 80.5f, 0f, 0f);
                transform.rotation = Quaternion.Euler(0f, (gyroInstance.GetGyroRotation().y + 0.3f) * 80.5f + 110f, 0f);
            }
            else
            {
                float _mouseVertical = -Input.GetAxis("Mouse Y");
                float _mouseHorizontal = Input.GetAxis("Mouse X");

                verticalRotation += _mouseVertical * sensitivity * Time.deltaTime;
                horizontalRotation += _mouseHorizontal * sensitivity * Time.deltaTime;

                verticalRotation = Mathf.Clamp(verticalRotation, -clamAngle, clamAngle);

                transform.rotation = Quaternion.Euler(transform.rotation.x, horizontalRotation + 110f, transform.rotation.z);
                player.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, transform.rotation.z);
            }
        }
    }
}