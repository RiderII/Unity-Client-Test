 using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    // store all players info in the client side.
    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();
    public static Dictionary<int, GameObject> obstacleStack = new Dictionary<int, GameObject>();

    public GameObject localPlayerPrefab;
    public GameObject playerPrefab;
    public GameObject localPlayerPrefabRigid;
    public GameObject playerPrefabRigid;
    public GameObject obstaclePrefab;
    public GameObject floatingTextPrefab;
    public GameObject conePrefab;
    public GameObject rockPrefab;
    public GameObject tirePrefab;
    public string sceneName;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            //ensure that only one instance of this class exists makes sense for every single client you only have one
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    public void Start()
    {
        PacketSend.SendIntoGame();
        sceneName = SceneManager.GetActiveScene().name;
    }

    public void SpawnPlayer(int _id, string _username, Vector3 _position, Quaternion _rotation)
    {
        GameObject _player;
        if (_id == Client.instance.myId)
        {
            if (sceneName == "Vaquita")
            {
                _player = Instantiate(localPlayerPrefab, _position, _rotation);
            }
            else
            {
                _player = Instantiate(localPlayerPrefabRigid, _position, _rotation);
            }
        }
        else
        {
            if (sceneName == "Vaquita")
            {
                _player = Instantiate(playerPrefab, _position, _rotation);
            }
            else
            {
                _player = Instantiate(playerPrefabRigid, _position, _rotation);
            }
        }

        _player.GetComponent<PlayerManager>().Initialize(_id, _username);
        players.Add(_id, _player.GetComponent<PlayerManager>());
    }

    public void SpawnObstacle(Vector3 _position)
    {
        if (sceneName != "200 metros" && sceneName != "500 metros")
        {
            Instantiate(obstaclePrefab, _position, obstaclePrefab.transform.rotation);
        }
    }

    public void SpawnObstacle2(int _id, Vector3 _position, Quaternion _rotation, string obstacleType = null)
    {
        if (!obstacleStack.ContainsKey(_id) && obstacleType != null)
        {
            GameObject stack = null;
            switch (obstacleType)
            {
                case "cone": stack = Instantiate(conePrefab, _position, _rotation); break;
                case "rock": stack = Instantiate(rockPrefab, _position, _rotation); break;
                case "tire": stack = Instantiate(tirePrefab, _position, _rotation); break;
            }

            obstacleStack.Add(_id, stack);
        }
        else if (obstacleStack.ContainsKey(_id))
        {
            obstacleStack[_id].transform.position = _position;
            obstacleStack[_id].transform.rotation = _rotation;
        }
    }

    public void SpawnFloatingPrefab(Vector3 _position, PlayerManager player)
    {
        Instantiate(floatingTextPrefab, new Vector3(player.transform.position.x,
            player.transform.position.y, player.transform.position.z),
            player.transform.rotation, player.transform);
    }
}
