using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateChange: MonoBehaviour
{
    public void ChangeState(string state)
    {
        switch (state)
        {
            case "Listo":
                {
                    PacketSend.SendReadyState();
                    break;
                } 
        }
    }
}
