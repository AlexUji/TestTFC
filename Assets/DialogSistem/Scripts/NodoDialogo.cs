using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NodoDialogo", menuName = "NodoDialogo", order = 1)]
public class NodoDialogo : ScriptableObject
{
    public string[] lineas;
    public NodoDialogo nextDialog;
    public Sprite portrait;
    public AudioClip sonido1;
    public AudioClip sonido2;

    public NodoDialogo(string[] l, NodoDialogo next, Sprite por)
    {
        lineas = l;
        nextDialog = next;
        portrait = por;
    }

    public NodoDialogo(string[] l,  Sprite por)
    {
        lineas = l;
        portrait = por;
    }

}


