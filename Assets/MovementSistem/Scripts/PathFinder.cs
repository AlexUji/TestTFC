using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinder 
{
    public List<OverlayTile> FindPath(OverlayTile starNode, OverlayTile endNode)
    {
        List<OverlayTile> openList = new List<OverlayTile>();
        List<OverlayTile> closeList = new List<OverlayTile>();


        openList.Add(starNode);

        while (openList.Count > 0)
        {
            OverlayTile currentNode = openList.OrderBy(x => x.F).First();

            openList.Remove(currentNode);
            closeList.Add(currentNode);

            if(currentNode == endNode)
            {
                return GetFinalPath(starNode, endNode);
            }

            var neighbourNodes = GetNeighboursNodes(currentNode);

            foreach (var neighbourNode in neighbourNodes)
            {
                if(neighbourNode.isBlocked || closeList.Contains(neighbourNode) || Math.Abs(currentNode.gridPosition.z - neighbourNode.gridPosition.z) > 1)
                {
                    continue;
                }

                neighbourNode.G = GetManhattanDistance(starNode, neighbourNode);
                neighbourNode.H = GetManhattanDistance(endNode, neighbourNode);

                neighbourNode.previusTile = currentNode;

                if (!openList.Contains(neighbourNode))
                {
                    openList.Add(neighbourNode);
                }
            }

        }
        return new List<OverlayTile>();
    }

    private List<OverlayTile> GetFinalPath(OverlayTile starNode, OverlayTile endNode)
    {
        List<OverlayTile> finalPath = new List<OverlayTile>();

        OverlayTile currentTile = endNode;

        while (currentTile != starNode)
        {
            finalPath.Add(currentTile);
            currentTile = currentTile.previusTile;
        }

        finalPath.Reverse();

        return finalPath;
    }

    private int GetManhattanDistance(OverlayTile starNode, OverlayTile neighbourNode)
    {
        return Math.Abs(starNode.gridPosition.x - neighbourNode.gridPosition.x) + Math.Abs(starNode.gridPosition.y - neighbourNode.gridPosition.y);
    }

    private List<OverlayTile> GetNeighboursNodes(OverlayTile currentNode)
    {
        var map = MapManager.Instance.map;

        List<OverlayTile> neighbours = new List<OverlayTile>();

        //Vecino Arriba
        Vector2Int locationToCheck = new Vector2Int(currentNode.gridPosition.x, currentNode.gridPosition.y + 1);

        if (map.ContainsKey(locationToCheck))
        {
            neighbours.Add(map[locationToCheck]);
        }

        //Vecino Abajo
        locationToCheck = new Vector2Int(currentNode.gridPosition.x, currentNode.gridPosition.y - 1);

        if (map.ContainsKey(locationToCheck))
        {
            neighbours.Add(map[locationToCheck]);
        }

        //Vecino Derecha
        locationToCheck = new Vector2Int(currentNode.gridPosition.x + 1, currentNode.gridPosition.y);

        if (map.ContainsKey(locationToCheck))
        {
            neighbours.Add(map[locationToCheck]);
        }

        //Vecino Izquiera
        locationToCheck = new Vector2Int(currentNode.gridPosition.x - 1, currentNode.gridPosition.y);

        if (map.ContainsKey(locationToCheck))
        {
            neighbours.Add(map[locationToCheck]);
        }

        return neighbours;
    }
}
