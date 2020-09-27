using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PacketHandle : MonoBehaviour
{
    public static void EnterLobby(Packet _packet) //read the value of the packets send from the server in the same order they were send
    {
        string _msg = _packet.ReadString();
        int _myId = _packet.ReadInt();

        Debug.Log($"Message from server: {_msg}");
        Client.instance.myId = _myId;

        PacketSend.RequestEnterLobbby();
    }

    public static void SendToLobby(Packet _packet)
    {
        int _clientId = _packet.ReadInt();
        string _playerName = _packet.ReadString();
        string _league = _packet.ReadString();

        LobbyGameManager.instance.SendToLobby(_clientId, _playerName, _league);
    }

    public static void AssignMiddleware(Packet _packet)
    {
        int _clientId = _packet.ReadInt();
        Client.instance.hasMiddleware = true;
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
        PlayerManager.statisticsFrame.transform.GetChild(6).GetComponent<TextMeshProUGUI>().text = System.DateTime.Now.ToString("hh.mm.ss.ffff");
        Debug.Log(System.DateTime.Now.ToString("hh.mm.ss.ffff"));
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
        GameManager.instance.SpawnFloatingPrefab(_position, GameManager.players[_id]);
    }

    public static void SpeedUp(Packet _packet)
    {
        int _id = _packet.ReadInt();
        bool speedUp = _packet.ReadBool();
        if (Client.instance.myId == _id && speedUp)
        {
            CameraController.audioSourcePedalo.Stop();
        }
        if (!speedUp)
        {
            CameraController.audioSourcePedaleoFaster.Stop();
            if (Client.instance.myId == _id)
            {
                CameraController.audioSourcePedalo.Play();
            }
        }
        CameraController.playPedaleoFaster = speedUp;
    }

    public static void UpdatePlayerStatistic(Packet _packet)
    {
        int playerId = _packet.ReadInt();
        float burned_calories = _packet.ReadFloat();
        float traveled_meters = _packet.ReadFloat();
        int points = _packet.ReadInt();
        float finalTime = _packet.ReadFloat();
        int placement = _packet.ReadInt();

        GameManager.players[playerId].burned_calories = burned_calories;
        GameManager.players[playerId].traveled_meters = traveled_meters;
        GameManager.players[playerId].points = points;
        GameManager.players[playerId].finalTime = finalTime;
        GameManager.players[playerId].placement = placement;
    }

    public static void UpdatePlayerSteps(Packet _packet)
    {
        int playerId = _packet.ReadInt();
        int steps = _packet.ReadInt();
        GameManager.players[playerId].steps = steps;
    }

    public static void UpdatePlayerLaps(Packet _packet)
    {
        int playerId = _packet.ReadInt();
        int laps = _packet.ReadInt();
        int totalLaps = 3;

        if (SceneManager.GetActiveScene().name != "4.6 kilómetros")
        {
            switch (Client.instance.levelSelected)
            {
                case "200 metros": totalLaps = Constants.twoHundredmeterLaps; break;
                case "500 metros": totalLaps = Constants.fiveHundredmeterLaps; break;
            }
        }

        PlayerManager.lapsFrame.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Vuelta " + laps.ToString() + " / " + totalLaps.ToString();
    }

    public static void UpdatePlayerPoints(Packet _packet)
    {
        int playerId = _packet.ReadInt();
        int points = _packet.ReadInt();
        GameManager.players[playerId].points = points;
    }

    public static void PlayerCollidedWithOtherPlayer(Packet _packet)
    {
        float speed = _packet.ReadFloat();
        bool collision = _packet.ReadBool();
        GameManager.players[Client.instance.myId].playBrakeCollision(speed, collision);
    }

    public static void ElementCollision(Packet _packet)
    {
        int _id = _packet.ReadInt();
        string elementTag = _packet.ReadString();
        int _collisions = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();

        GameManager.players[_id].SetCollisions(_collisions);
        CameraController.collisionPosition = _position;
    }

    public static void ObstacleSpawned(Packet _packet)
    {
        Vector3 _position = _packet.ReadVector3();

        if (GameManager.instance != null) {
            GameManager.instance.SpawnObstacle(_position);
        }
    }

    public static void PlayerFinishedGame(Packet _packet)
    {
        int _id = _packet.ReadInt();
        float _speed = _packet.ReadFloat();

        GameManager.players[_id].playBrake(_speed);
        GameManager.players[_id].finishedGame = true;
    }
}
