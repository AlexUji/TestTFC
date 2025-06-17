using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RangeFinder
{
    public List<OverlayTile> GetTilesInRange(OverlayTile startTile, int range)
    {
        var inRangeTiles = new List<OverlayTile>();
        int stepCount = 0;

        inRangeTiles.Add(startTile);

        var tileForPStep = new List<OverlayTile>();
        tileForPStep.Add(startTile);

        while(stepCount < range)
        {
            var surroundigTiles = new List<OverlayTile>();

            foreach (var tile in tileForPStep)
            {
                surroundigTiles.AddRange(MapManager.Instance.GetNeighboursNodes(tile, new List<OverlayTile>()));
            }

            inRangeTiles.AddRange(surroundigTiles);
            tileForPStep = surroundigTiles.Distinct().ToList();
            stepCount++;
        }
        inRangeTiles.Remove(startTile);
        return inRangeTiles.Distinct().ToList();
    }
}
