using System.Collections.Generic;
using UnityEngine;

public class CharacterInfo : MonoBehaviour
{
    public Vector3Int SpawnPosition = Vector3Int.zero;

    public OverlayTile activeTile;

    public Sprite portrait;

    public Team team = Team.None;
    public UnitType type;

    public bool haveMoved = false;
    public bool haveAttacked = false;
    public bool isFocused = false;
    public bool haveActions {  get { return !haveMoved || !haveAttacked; }}
    public string characterName;
    
    public List<Ability> habilities;
    public GameObject menu;
    public List<Sprite> menuSprites;

    public int enemyCount;

    ///STATS///
    public int level;
    
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
        
        //enemy.transform.GetChild(2).GetComponent<golpe>().attacker = transform.GetComponent<CharacterInfo>();
        enemy.currentHP -= attack - enemy.defense;
        if(enemy.currentHP <= 0)
        {
            EnemySlayed(enemy);
        }
        else
        {
            enemy.transform.GetChild(2).gameObject.SetActive(true);
        }
        haveAttacked = true;
        
    }

    public void IAbasicAttack(CharacterInfo enemy)
    {
        
        enemy.currentHP -= attack - enemy.defense;
        if (enemy.currentHP <= 0)
        {
            //Animación muerte
            IAEnemySlayed(enemy);
            haveAttacked = true;
        }
        else
        {
            enemy.transform.GetChild(2).gameObject.SetActive(true);
        }
        
    }

    public void EnemySlayed(CharacterInfo enemy)
    {
        //Debug.Log(enemy.gameObject.name + "Estaaaa moooortooo");

        enemy.activeTile.isBlocked = false;
        Destroy(enemy.gameObject);
        enemyCount++;
        if (enemyCount >= 3)
        {
            //Mostrar graficamente
            int prelvl = level;
            level++;
            Debug.Log("Has pasado de " + prelvl + "ha " + level);
            enemyCount = 0;
        }
        //TurnSistem.Instance.InitialiceIAInfo();
        
        TurnSistem.Instance.UpdateInfluence(); //Actualiza el mapa de influencia
        if(TurnSistem.Instance.AllyActionsPerTurn == 1 && haveMoved == true)
        {
            TurnSistem.Instance.SelectTroopInDeath();
        }
        haveAttacked = true;

    }

    public void IAEnemySlayed(CharacterInfo enemy)
    {
        //Debug.Log(enemy.gameObject.name + "Estaaaa moooortooo");
        enemy.activeTile.isBlocked = false;
        Destroy(enemy.gameObject);
        TurnSistem.Instance.UpdateInfluence();

    }
}
