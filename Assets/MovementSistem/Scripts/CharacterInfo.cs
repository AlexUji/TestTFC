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
    
    public List<Ability> habilities;
    public GameObject menu;
    public List<Sprite> menuSprites;


    ///STATS///
    public int level;
    private int enemyCount;
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
        if(enemy.currentHP <= 0)
        {
            //Animación muerte
            Destroy(enemy.gameObject);
        }
    }

    public void EnemySlayed()
    {
        enemyCount++;
        if (enemyCount >= 3)
        {
            //Mostrar graficamente
            int prelvl = level;
            level++;
            Debug.Log("Has pasado de " + prelvl + "ha " + level);
            enemyCount = 0;
        }
    }
}
