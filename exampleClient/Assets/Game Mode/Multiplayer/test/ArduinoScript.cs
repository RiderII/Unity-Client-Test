using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class ArduinoScript : MonoBehaviour
{
    //public float speed;
    //private float amountToMove;

   
    SerialPort sp = new SerialPort("COM5",9600);

    // Start is called before the first frame update
    void Start()
    {
        sp.Open();
        sp.ReadTimeout = 1;
    }

    // Update is called once per frame
    void Update()
    {
        //amountToMove = speed * Time.deltaTime;
        if (sp.IsOpen)
        {
            try
            {
                MoveObject(sp.ReadByte());

            }
            catch (System.Exception)
            {

            }
        }
    }

     void MoveObject(int direction)
    {
        //Debug.Log(direction);

        if (direction == 1)
        {
            print("Iman detectado");
            //transform.Translate(Vector3.left* amountToMove, Space.World);
        }
        if (direction == 2)
        {
            print("nada"); 
            //transform.Translate(Vector3.right* amountToMove, Space.World);
        }
    }
}
