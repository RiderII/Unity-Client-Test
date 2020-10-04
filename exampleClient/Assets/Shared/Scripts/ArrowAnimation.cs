using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowAnimation : MonoBehaviour
{
    public float itemRotationSpeed = 50f;
    public float itemBobSpeed = 2f;
    private Vector3 basePosition;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, itemRotationSpeed * Time.deltaTime, Space.World);
        transform.position = new Vector3(0f, Mathf.Sin(Time.time * itemBobSpeed), 0.25f);
    }
}
