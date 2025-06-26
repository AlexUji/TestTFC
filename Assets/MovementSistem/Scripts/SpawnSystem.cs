using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSystem : MonoBehaviour
{
    public List<GameObject> allieTeam;

    public List<GameObject> enemyTeam;

    


    public void SpawnAllies(Dictionary<Vector2Int, OverlayTile> map)
    {
        foreach (var character in allieTeam)
        {
            character.GetComponent<CharacterInfo>().team = Team.Ally;
            //TurnSistem.Instance.AllyList.Add(character.GetComponent<CharacterInfo>());
            foreach (var tile in map)
            {
                if (tile.Value.gridPosition.Equals(character.GetComponent<CharacterInfo>().SpawnPosition))
                {
                    character.transform.position = new Vector3(tile.Value.transform.position.x, tile.Value.transform.position.y, tile.Value.transform.position.z);
                  
                    character.GetComponent<CharacterInfo>().activeTile = tile.Value;
                    CharacterInfo newCharacter = character.GetComponent<CharacterInfo>();
                    newCharacter = Instantiate(character, TurnSistem.Instance.AllyTeam.transform).GetComponent<CharacterInfo>();
                    tile.Value.characterInTile = newCharacter;
                    tile.Value.isBlocked = true;
                    tile.Value.influence = 1;
                    RangeFinder rangeFinder = new RangeFinder();
                    List<OverlayTile> neighboursTiles = MapManager.Instance.GetNeighboursNodes(tile.Value, rangeFinder.GetTilesInRange(tile.Value,1));
                    foreach (OverlayTile nTile in neighboursTiles)
                    {
                        if(nTile.influence < 1 && nTile.influence != 1 && nTile.influence != -1)
                        {
                            nTile.influence += 0.25f;
                            if(nTile.influence > 1)
                                nTile.influence = 1;
                        }
                        
                    }
                    break;
                }
            }
        }
    }

    public void SpawnEnemys(Dictionary<Vector2Int, OverlayTile> map)
    {
        foreach (var character in enemyTeam)
        {
            character.GetComponent<CharacterInfo>().team = Team.Enemy;
            //TurnSistem.Instance.EnemyList.Add(character.GetComponent<CharacterInfo>());
            foreach (var tile in map)
            {
                if (tile.Value.gridPosition.Equals(character.GetComponent<CharacterInfo>().SpawnPosition))
                {
                    character.transform.position = new Vector3(tile.Value.transform.position.x, tile.Value.transform.position.y, tile.Value.transform.position.z);

                    character.GetComponent<CharacterInfo>().activeTile = tile.Value;
                    CharacterInfo newCharacter = character.GetComponent<CharacterInfo>();
                    newCharacter = Instantiate(character, TurnSistem.Instance.EnemyTeam.transform).GetComponent<CharacterInfo>();
                    tile.Value.characterInTile = newCharacter;
                    tile.Value.isBlocked = true;
                    tile.Value.influence = -1;
                    RangeFinder rangeFinder = new RangeFinder();
                    List<OverlayTile> neighboursTiles = MapManager.Instance.GetNeighboursNodes(tile.Value, rangeFinder.GetTilesInRange(tile.Value, 1));
                    foreach (OverlayTile nTile in neighboursTiles)
                    {
                        if (nTile.influence > -1 && nTile.influence != 1 && nTile.influence != -1)
                        {
                            nTile.influence -= 0.25f;
                            if (nTile.influence < -1)
                                nTile.influence = -1;
                        }

                    }
                    break;
                }
            }
        }
    }
}
