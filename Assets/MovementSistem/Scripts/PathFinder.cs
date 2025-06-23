using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinder 
{
    public List<OverlayTile> FindPath(OverlayTile starNode, OverlayTile endNode, List<OverlayTile> tilesInRange)
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

            var neighbourNodes = MapManager.Instance.GetNeighboursNodes(currentNode, tilesInRange);

            foreach (var neighbourNode in neighbourNodes)
            {
                if(neighbourNode.isBlocked || closeList.Contains(neighbourNode))
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
       /* Debug.Log("Start node "+starNode.name);
        Debug.Log("Neighbour node "+neighbourNode.name);*/

        return Math.Abs(starNode.gridPosition.x - neighbourNode.gridPosition.x) + Math.Abs(starNode.gridPosition.y - neighbourNode.gridPosition.y);
    }

}
