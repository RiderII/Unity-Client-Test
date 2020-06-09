using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Challenge
{
    public string Descripcion;
    public int Puntos;

    public Challenge() { }

    public Challenge(string descripcion, int puntos)
    {
        Descripcion = descripcion;
        Puntos = puntos;
    }
}
