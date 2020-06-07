using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    SerialPort sp = new SerialPort("COM2", 9600);

    private void Start()
    {
        sp.Open();
        sp.ReadTimeout = 1;
    }

    private void FixedUpdate()
    {
        SendInputToServer();
       
        if (sp.IsOpen)
        {
            try
            {
                SendInputIot(sp.ReadByte());

            }
            catch (System.Exception)
            {

            }
        }
    }

    private void SendInputIot(int direction)
    {
        bool[] _inputs = new bool[] { };
        if (direction == 1)
        {
           _inputs = new bool[] {true,false,false,false };
        }
        if (direction == 2)
        {
            _inputs = new bool[] { false, true, false, false };
        }
        if (direction == 3)
        {
            _inputs = new bool[] { false, false, true, false };
        }
        if (direction == 4)
        {
            _inputs = new bool[] { false, false, false, true };
        }
        PacketSend.PlayerMovement(_inputs);
    }

    private void SendInputToServer()
    {
        bool[] _inputs = new bool[]
        {
            Input.GetKey(KeyCode.W),
            Input.GetKey(KeyCode.S),
            Input.GetKey(KeyCode.A),
            Input.GetKey(KeyCode.D)
        };

        PacketSend.PlayerMovement(_inputs);
    }
}
