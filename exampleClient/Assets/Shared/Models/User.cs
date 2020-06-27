using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class User 
{
    public string ID;
    public string email;
    public string username;
    public string mode;

    public User() { }
    public User(string id, string username, string email) {
        ID = id;
        this.username = username;
        this.email = email;
        mode = "";
    }
    public User(string id, string username, string email, string mode)
    {
        ID = id;
        this.username = username;
        this.email = email;
        this.mode = mode;
    }
}
