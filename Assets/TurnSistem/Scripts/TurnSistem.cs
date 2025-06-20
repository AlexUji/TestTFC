using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnSistem : MonoBehaviour
{
    public static TurnSistem Instance;
    public GameObject AllyTeam;
    public GameObject EnemyTeam;
    public bool AllyTurn = true;
    public bool EnemyTurn = false;
    public int AllyActionsPerTurn = 0;
    public int EnemyActionsPerTurn = 0;



    void Awake()
    {
        Instance = this;
    }

    public void ResetTeams()
    {
        foreach (GameObject character in AllyTeam.transform)
        {
            Destroy(character);
        }

        foreach (GameObject character in EnemyTeam.transform)
        {
            Destroy(character);
        }
    }

    public void InitialzeTeams()
    {
        AllyActionsPerTurn = AllyTeam.transform.childCount;
        EnemyActionsPerTurn = EnemyTeam.transform.childCount;

    }

    public void EndAction(CharacterInfo character)
    {
        if(character.haveAttacked && character.haveMoved)
        {
            character.transform.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f, 1);

            if(character.team == Team.Ally)
            {
                AllyActionsPerTurn--;
                if (AllyActionsPerTurn <= 0)
                {
                    AllyTurn = false;
                    EnemyTurn = true;
                    foreach (Transform c in AllyTeam.transform)
                    {
                        c.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1);
                    }
                    Debug.Log("Turno enemigo");
                    AllyActionsPerTurn = AllyTeam.transform.childCount;
                }
            }
            else
            {
                EnemyActionsPerTurn--;
                if (EnemyActionsPerTurn <= 0)
                {
                    AllyTurn = true;
                    EnemyTurn = false;
                    foreach (Transform c in EnemyTeam.transform)
                    {
                        c.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1);
                    }
                    Debug.Log("Tu turno");
                    EnemyActionsPerTurn = EnemyTeam.transform.childCount;
                }
            }

        }
    }

}
