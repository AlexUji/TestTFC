using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnSistem : MonoBehaviour
{
    public static TurnSistem Instance;
    private RangeFinder RangeFinder;
    public GameObject AllyTeam;
    public GameObject EnemyTeam;
    public GameObject Map;
    public bool AllyTurn = true;
    public bool EnemyTurn = false;
    public int AllyActionsPerTurn = 0;
    public int EnemyActionsPerTurn = 0;
    private int idTroop = 0;
    public IAInfo IAInfo;



    void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        RangeFinder = new RangeFinder();
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
        if(!character.haveActions)
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
                        c.GetComponent<CharacterInfo>().haveAttacked = false;
                        c.GetComponent<CharacterInfo>().haveMoved = false;

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
                        c.GetComponent<CharacterInfo>().haveAttacked = false;
                        c.GetComponent<CharacterInfo>().haveMoved = false;
                    }
                    Debug.Log("Tu turno");
                    EnemyActionsPerTurn = EnemyTeam.transform.childCount;
                }
            }

        }
    }

    public void InitialiceIAInfo()
    {
        idTroop = 0;
        IAInfo.allyTeam = AllyTeam;
        IAInfo.enemyTeam = EnemyTeam;
    } 

    public void SelectTroop()
    {
        IAInfo.selectedTroop = EnemyTeam.transform.GetChild(idTroop).GetComponent<CharacterInfo>();
        idTroop++;

    }

    public void UpdateInfluenceMovement(OverlayTile posIni, OverlayTile posF)
    {
        float substractInf;
        float sumInfluence = 0;
        float finalInfluence = 0;

        if (posIni.characterInTile.team == Team.Ally)
        {
            substractInf = -0.25f;
            finalInfluence = 1;
        }           
        else
        {
            substractInf = 0.25f;
            finalInfluence = -1;
        }


        List<OverlayTile> neighboursTiles = MapManager.Instance.GetNeighboursNodes(posIni, RangeFinder.GetTilesInRange(posIni, 1));
        foreach (OverlayTile nTile in neighboursTiles)
        {
            if (nTile.influence != -1 && nTile.influence != 1)
            {
                nTile.influence += substractInf;

                if (nTile.influence < -1)
                    nTile.influence = -1;
                else if (nTile.influence > 1)
                    nTile.influence = 1;

               
            }
            sumInfluence += nTile.influence;
        }
        if(sumInfluence > 1)
            sumInfluence = 1;
        else if (sumInfluence < -1)
            sumInfluence = -1;

        posIni.influence = sumInfluence;

        neighboursTiles = MapManager.Instance.GetNeighboursNodes(posF, RangeFinder.GetTilesInRange(posF, 1));

        foreach (OverlayTile nTile in neighboursTiles)
        {
            if (nTile.influence != -1 && nTile.influence != 1)
            {
                nTile.influence += substractInf;

                if (nTile.influence < -1)
                    nTile.influence = -1;
                else if (nTile.influence > 1)
                    nTile.influence = 1;

            }            
        }
        posF.influence = finalInfluence;

    }

    public void UpdateInfluenceUnitSlayed(OverlayTile posIni)
    {
        posIni.influence = 0;
        float sumInfluence = 0;

        List<OverlayTile> neighboursTiles = MapManager.Instance.GetNeighboursNodes(posIni, RangeFinder.GetTilesInRange(posIni, 1));
        foreach (OverlayTile nTile in neighboursTiles)
        {
            sumInfluence += nTile.influence;
        }
           

        if (sumInfluence > 1)
            sumInfluence = 1;
        else if (sumInfluence < -1)
            sumInfluence = -1;

        posIni.influence = sumInfluence;
    }

    public void UpdateInfluence()
    {
        foreach (Transform tile in Map.transform)
        {
           if(tile.GetComponent<OverlayTile>().characterInTile != null)
            {
                if(tile.GetComponent<OverlayTile>().characterInTile.team == Team.Ally)
                {
                    tile.GetComponent<OverlayTile>().influence = 1;
                    List <OverlayTile> neighboursTiles = MapManager.Instance.GetNeighboursNodes(tile.GetComponent<OverlayTile>(), RangeFinder.GetTilesInRange(tile.GetComponent<OverlayTile>(), 1));
                    foreach (OverlayTile nTile in neighboursTiles)
                    {
                        if (nTile.influence != 1 && nTile.influence != -1)
                        {
                            nTile.influence += 0.25f;
                            if (nTile.influence > 1)
                                nTile.influence = 1;
                        }

                    }
                }
                else if (tile.GetComponent<OverlayTile>().characterInTile.team == Team.Ally)
                {
                    tile.GetComponent<OverlayTile>().influence = -1;
                    List<OverlayTile> neighboursTiles = MapManager.Instance.GetNeighboursNodes(tile.GetComponent<OverlayTile>(), RangeFinder.GetTilesInRange(tile.GetComponent<OverlayTile>(), 1));
                    foreach (OverlayTile nTile in neighboursTiles)
                    {
                        if (nTile.influence != 1 && nTile.influence != -1)
                        {
                            nTile.influence -= 0.25f;
                            if (nTile.influence < -1)
                                nTile.influence = -1;
                        }

                    }
                }
            }
        }
    }

    public void UpdateAmountOfInfluence()
    {
        IAInfo.amountOfInfluence = 0;
        foreach (Transform tile in Map.transform)
        {
            IAInfo.amountOfInfluence += tile.GetComponent<OverlayTile>().influence;
        }
    }
}
