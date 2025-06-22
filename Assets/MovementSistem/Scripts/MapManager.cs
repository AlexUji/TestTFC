using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    private static MapManager _instance;
    public static MapManager Instance {  get { return _instance; } }

    public OverlayTile overlayTilePrefab;
    public GameObject overlayContainer;
    public SpawnSystem spawnSystem;

    public Dictionary<Vector2Int, OverlayTile> map;

    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    void Start()
    {
       
        var tileMap = gameObject.GetComponentInChildren<Tilemap>();
        map = new Dictionary<Vector2Int, OverlayTile>();

        BoundsInt bounds = tileMap.cellBounds;

        for (int z = bounds.max.z; z >= bounds.min.z; z--)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                for (int x = bounds.min.x; x < bounds.max.x; x++)
                {
                    var tileLocation = new Vector3Int(x, y, z);

                    var tileKey = new Vector2Int(x, y);

                    if (tileMap.HasTile(tileLocation) && !map.ContainsKey(tileKey))
                    {
                        
                        var overlayTile = Instantiate(overlayTilePrefab, overlayContainer.transform);
                        var cWorldPos = tileMap.GetCellCenterWorld(tileLocation);

                        overlayTile.transform.position = new Vector3(cWorldPos.x, cWorldPos.y, cWorldPos.z + 1);
                        overlayTile.GetComponent<SpriteRenderer>().sortingOrder = tileMap.GetComponent<TilemapRenderer>().sortingOrder;
                        overlayTile.GetComponent<SpriteRenderer>().enabled = false;
                        
                        overlayTile.gridPosition = tileLocation;
                        map.Add(tileKey, overlayTile);
                    }
                }
            }
        }

        spawnSystem.SpawnAllies(map);
        spawnSystem.SpawnEnemys(map);
        TurnSistem.Instance.InitialzeTeams();
        foreach (var tile in map)
        {
            Debug.Log(tile.Value.influence);
        }

    }

    public List<OverlayTile> GetNeighboursNodes(OverlayTile currentNode, List<OverlayTile> tilesInRange)
    {
        Dictionary<Vector2Int, OverlayTile> tilesToSearch = new Dictionary<Vector2Int, OverlayTile>();

        if(tilesInRange.Count > 0)
        {
            foreach (var tile in tilesInRange)
            {
                tilesToSearch.Add(tile.grid2DPosition, tile);
            }
        }
        else
        {
            tilesToSearch = map;
        }

        List<OverlayTile> neighbours = new List<OverlayTile>();

        //Vecino Arriba
        Vector2Int locationToCheck = new Vector2Int(currentNode.gridPosition.x, currentNode.gridPosition.y + 1);

        if (tilesToSearch.ContainsKey(locationToCheck))
        {
            if(Math.Abs(currentNode.gridPosition.z - tilesToSearch[locationToCheck].gridPosition.z) <= 1)
                neighbours.Add(tilesToSearch[locationToCheck]);
        }

        //Vecino Abajo
        locationToCheck = new Vector2Int(currentNode.gridPosition.x, currentNode.gridPosition.y - 1);

        if (tilesToSearch.ContainsKey(locationToCheck))
        {
            if (Math.Abs(currentNode.gridPosition.z - tilesToSearch[locationToCheck].gridPosition.z) <= 1)
                neighbours.Add(tilesToSearch[locationToCheck]);
        }

        //Vecino Derecha
        locationToCheck = new Vector2Int(currentNode.gridPosition.x + 1, currentNode.gridPosition.y);

        if (tilesToSearch.ContainsKey(locationToCheck))
        {
            if (Math.Abs(currentNode.gridPosition.z - tilesToSearch[locationToCheck].gridPosition.z) <= 1)
                neighbours.Add(tilesToSearch[locationToCheck]);
        }

        //Vecino Izquiera
        locationToCheck = new Vector2Int(currentNode.gridPosition.x - 1, currentNode.gridPosition.y);

        if (tilesToSearch.ContainsKey(locationToCheck))
        {
            if (Math.Abs(currentNode.gridPosition.z - tilesToSearch[locationToCheck].gridPosition.z) <= 1)
                neighbours.Add(tilesToSearch[locationToCheck]);
        }

        return neighbours;
    }

}
