using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class PacketHandle : MonoBehaviour
{
    public static void EnterLobby(Packet _packet) //read the value of the packets send from the server in the same order they were send
    {
        string _msg = _packet.ReadString();
        int _myId = _packet.ReadInt();

        Debug.Log($"Message from server: {_msg}");
        Client.instance.myId = _myId;

        // Send packet back to the server
        PacketSend.RequestEnterLobbby();

        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
    }

    public static void SendToLobby(Packet _packet)
    {
        int _clientId = _packet.ReadInt();
        string _playerName = _packet.ReadString();
        string _category = _packet.ReadString();

        LobbyGameManager.instance.SendToLobby(_clientId, _playerName, _category);
    }

    public static void SendReadyState(Packet _packet)
    {
        int _clientId = _packet.ReadInt();
        LobbyGameManager.clientsInLobby[_clientId].lobbyState = "Listo";
    }

    public static void SpawnPlayer(Packet _packet)
    {
        int _id = _packet.ReadInt();
        string _username = _packet.ReadString();
        Vector3 _position = _packet.ReadVector3();
        Quaternion _rotation = _packet.readQuaternion();

        GameManager.instance.SpawnPlayer(_id, _username, _position, _rotation);
    }

    public static void PlayerPosition(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();

        if (GameManager.players.ContainsKey(_id))
        {
            GameManager.players[_id].transform.position = _position;
        }
    }

    public static void RestartPlayerPosition(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();

        GameManager.players[_id].transform.position = _position;
        GameManager.players[_id].RestartProperties();
        Debug.Log($"Player>: {GameManager.players[_id].username} sent to position { GameManager.players[_id].transform.position}");
    }

    public static void PlayerRotation(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Quaternion _rotation = _packet.readQuaternion();

        if (GameManager.players.ContainsKey(_id))
        {
            GameManager.players[_id].transform.rotation = _rotation;
        }
    }

    public static void PlayerDisconnected(Packet _packet)
    {
        int _id = _packet.ReadInt();

        Destroy(GameManager.players[_id].gameObject);
        GameManager.players.Remove(_id);
    }

    public static void PlayerCollided(Packet _packet)
    {
        int _id = _packet.ReadInt();
        int _collisions = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();

        GameManager.players[_id].SetCollisions(_collisions);
        CameraController.collisionPosition = _position;
        CameraController.playVaquitaMu = true;
    }

    public static void ObstacleSpawned(Packet _packet)
    {
        Vector3 _position = _packet.ReadVector3();

        GameManager.instance.SpawnObstacle(_position);
    }

    public static void PlayerFinishedGame(Packet _packet)
    {
        int _id = _packet.ReadInt();

        GameManager.players[_id].finishedGame = true;
    }
}
