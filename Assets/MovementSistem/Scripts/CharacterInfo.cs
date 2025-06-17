using System.Collections.Generic;
using UnityEngine;

public class CharacterInfo : MonoBehaviour
{
    public Vector3Int SpawnPosition = Vector3Int.zero;
    public OverlayTile activeTile;
    public Sprite portrait;
    public bool haveMoved = false;
    public bool haveAttacked = false;
    public bool isFocused = false;
    public string characterName;
    
    public List<Habilitiy> habilities;
    public GameObject menu;
    public List<Sprite> menuSprites;


    ///STATS///
    public int MaxHP;
    public int MaxMP;
    public int currentHP;
    public int currentMP;
    public int movementRange;
    public int atackRange;
    public int attack;
    public int defense;
    public int magicAtack;
    public int magicDefense;


    public void basicAttack(CharacterInfo enemy)
    {
        enemy.currentHP -= attack - enemy.defense;
    }
}
