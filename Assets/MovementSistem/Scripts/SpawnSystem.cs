using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSystem : MonoBehaviour
{
    public List<GameObject> allieTeam;

    public void SpawnAllies(Dictionary<Vector2Int, OverlayTile> map)
    {
        foreach (var character in allieTeam)
        {
            foreach (var tile in map)
            {
                if (tile.Value.gridPosition.Equals(character.GetComponent<CharacterInfo>().SpawnPosition))
                {
                    character.transform.position = new Vector3(tile.Value.transform.position.x, tile.Value.transform.position.y, tile.Value.transform.position.z);
                  
                    character.GetComponent<CharacterInfo>().activeTile = tile.Value;
                    CharacterInfo newCharacter = character.GetComponent<CharacterInfo>();
                    newCharacter = Instantiate(character).GetComponent<CharacterInfo>();
                    tile.Value.characterInTile = newCharacter;
                    break;
                }
            }
        }
    }
}
