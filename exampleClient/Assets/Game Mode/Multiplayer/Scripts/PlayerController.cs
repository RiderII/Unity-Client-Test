using System.Collections;
using System.Collections.Generic;
//using System.IO.Ports;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int nextUpdate = 1;
    private int frames = 0;

    private void Start()
    {
        Time.timeScale = 1;
        //sp.Open();
        //sp.ReadTimeout = 1;
    }

    private void Update()
    {

        SendInputToServer();

        

        //if (sp.IsOpen)
        //{
        //    try
        //    {
        //        SendInputIot(sp.ReadByte());

        //    }
        //    catch (System.Exception)
        //    {

        //    }
        //}
    }

    //private void SendInputIot(int direction)
    //{
    //    bool[] _inputs = new bool[] { };
    //    if (direction == 1)
    //    {
    //       _inputs = new bool[] {true,false,false,false };
    //    }
    //    if (direction == 2)
    //    {
    //        _inputs = new bool[] { false, true, false, false };
    //    }
    //    if (direction == 3)
    //    {
    //        _inputs = new bool[] { false, false, true, false };
    //    }
    //    if (direction == 4)
    //    {
    //        _inputs = new bool[] { false, false, false, true };
    //    }
    //    PacketSend.PlayerMovement(_inputs);
    //}

    private void SendInputToServer()
    {
        frames++;
        if (Time.time >= nextUpdate)
        {
            nextUpdate = Mathf.FloorToInt(Time.time) + 1;
            //Debug.Log(frames);
        }


        bool[] _inputs = null;

        if (!Client.instance.hasMiddleware)
        {
            if (GameManager.players.ContainsKey(Client.instance.myId) && !GameManager.players[Client.instance.myId].finishedGame)
            {
                _inputs = new bool[]
                {
                Input.GetKey(KeyCode.W),
                false,
                false,
                false
                };
            }
            else
            {
                _inputs = new bool[]
               {
                false,
                false,
                false,
                false
               };
            }

            PacketSend.PlayerMovement(_inputs);
        }
    }
}
