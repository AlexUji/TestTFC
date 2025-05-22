using System.Collections.Generic;
using UnityEngine;

public class CharacterInfo : MonoBehaviour
{
    public OverlayTile activeTile;
    public Sprite portrait;
    public bool haveMoved = false;
    public bool haveAttacked = false;
    public string characterName;
    public int MaxHP;
    public int MaxMP;
    public int currentHP;
    public int currentMP;
    public int movementRange;
    public int atackRange;
    public List<Habilitiy> habilities; 
}
