using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacketSend : MonoBehaviour
{
    private static void SendTCPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.tcp.SendData(_packet);
    }

    private static void SendUDPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.udp.SendData(_packet);
    }

    #region Packets
    public static void RequestEnterLobbby()
    {
        using (Packet _packet = new Packet((int)ClientPackets.requestEnteredLobby))
        {
            _packet.Write(Client.instance.myId); //the server can confirm that the client claimed the correct Id.
            _packet.Write(Client.instance.userName);
            _packet.Write(Client.instance.league);
            _packet.Write(Client.instance.levelSelected);

            SendTCPData(_packet);
        }
    }

    public static void SendReadyState()
    {
        using (Packet _packet = new Packet((int)ClientPackets.sendReadyState))
        {
            _packet.Write(Client.instance.myId);

            SendTCPData(_packet);
        }
    }

    public static void SendIntoGame()
    {
        using (Packet _packet = new Packet((int)ClientPackets.sendToGame))
        {
            _packet.Write(Client.instance.myId);
            _packet.Write(Client.instance.userName);

            SendTCPData(_packet);
        }
    }

    public static void PlayerMovement(bool[] _inputs)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerMovement))
        {
            _packet.Write(_inputs.Length);
            foreach (bool _input in _inputs)
            {
                _packet.Write(_input);
            }

            if (GameManager.players.ContainsKey(Client.instance.myId))
            {
                _packet.Write(GameManager.players[Client.instance.myId].transform.rotation);
            }

            SendUDPData(_packet);
        }
    }

    public static void RequestGameRestart()
    {
        using (Packet _packet = new Packet((int)ClientPackets.restartScene))
        {
            _packet.Write(Client.instance.myId);

            SendTCPData(_packet);
        }
    }
        
    #endregion
}
