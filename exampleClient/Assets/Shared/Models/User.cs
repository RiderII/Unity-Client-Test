using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class User 
{
    public string ID { get; set; }
    public string username;
    public string mode;

    public User() { }
    public User(string id, string username) {
        ID = id;
        this.username = username;
        mode = "";
    }
}
