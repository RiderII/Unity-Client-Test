using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public int id;
    public string username;
    public float collisions;
    public float traveled_kilometers;
    public float burned_calories;

    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;
    }

    public void SetCollisions(float _collision)
    {
        collisions = _collision;
    }
}
