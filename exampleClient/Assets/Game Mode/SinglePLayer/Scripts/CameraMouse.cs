using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMouse : MonoBehaviour
{

    public float mouseSensitivity = 100.0f;
    public float clampAngle = 80.0f;
    private float rotX = 0.0f; // rotation around the right/x axis

    // Start is called before the first frame update
    void Start()
    {
        Vector3 rot = transform.localRotation.eulerAngles;
        rotX = rot.x;
    }

    // Update is called once per frame
    void Update()
    {
        float _mouseHorizontal = Input.GetAxis("Mouse X");
        rotX += _mouseHorizontal * mouseSensitivity * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(0f, rotX, 0f);
    }
}