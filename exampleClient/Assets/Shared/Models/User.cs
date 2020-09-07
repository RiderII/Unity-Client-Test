using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class User 
{
    public string ID;
    public int userServerId;
    public string email;
    public string username;
    public string league;
    public string lobbyState = "Pendiente";
    public string mode;
    public float weight;
    public float bikeWheelDiameter; 

    public User() { }

    public User(int userServerId, string username, string league) { // for lobby
        this.userServerId = userServerId;
        this.username = username;
        this.league = league;
    }

    public User(string id, string username, string email) {
        ID = id;
        this.username = username;
        this.email = email;
        mode = "";
        weight = 0;
        bikeWheelDiameter = 0;
    }
    public User(string id, string username, string email, string mode)
    {
        ID = id;
        this.username = username;
        this.email = email;
        this.mode = mode;
    }
}
