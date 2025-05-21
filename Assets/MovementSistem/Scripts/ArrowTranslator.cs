using UnityEngine;

public class ArrowTranslator
{
   public enum ArrowDiewctions
    {
        None = 0,
        Up = 1,
        Down = 2,
        Left = 3,
        Right = 4,
        TopRight = 5,
        BottomRight = 6,
        TopLeft = 7,
        BottomLeft = 8,
        UpFinished = 9,
        DownFinished = 10,
        LeftFinished = 11,
        RightFinished = 12,
    }

    public ArrowDiewctions TranslateDirections(OverlayTile previuosTile, OverlayTile currenTile, OverlayTile futureTile)
    {
        bool isFinal = futureTile == null;

        Vector2Int pastDirection =  previuosTile != null ? currenTile.grid2DPosition - previuosTile.grid2DPosition : new Vector2Int(0,0);

        Vector2Int futureDirection = futureTile != null ? futureTile.grid2DPosition - currenTile.grid2DPosition : new Vector2Int(0, 0);

        Vector2Int direction = pastDirection != futureDirection ? pastDirection + futureDirection : futureDirection;

        if(direction == new Vector2Int(0,1) && !isFinal)
        {
            return ArrowDiewctions.Up;
        }

        if (direction == new Vector2Int(0, -1) && !isFinal)
        {
            return ArrowDiewctions.Down;
        }

        if (direction == new Vector2Int(1, 0) && !isFinal)
        {
            return ArrowDiewctions.Right;
        }

        if (direction == new Vector2Int(-1, 0) && !isFinal)
        {
            return ArrowDiewctions.Left;
        }

        if (direction == new Vector2Int(1, 1))
        {
          if(pastDirection.y < futureDirection.y)
            {
                return ArrowDiewctions.BottomLeft;
            }
            else
            {
                return ArrowDiewctions.TopRight;
            }
        }

        if (direction == new Vector2Int(-1, 1))
        {
            if (pastDirection.y < futureDirection.y)
            {
                return ArrowDiewctions.BottomRight;
            }
            else
            {
                return ArrowDiewctions.TopLeft;
            }
        }

        if (direction == new Vector2Int(1, -1))
        {
            if (pastDirection.y > futureDirection.y)
            {
                return ArrowDiewctions.TopLeft;
            }
            else
            {
                return ArrowDiewctions.BottomRight;
            }
        }

        if (direction == new Vector2Int(-1, -1))
        {
            if (pastDirection.y > futureDirection.y)
            {
                return ArrowDiewctions.TopRight;
            }
            else
            {
                return ArrowDiewctions.BottomLeft;
            }
        }

        if (direction == new Vector2Int(0, 1) && isFinal)
        {
            return ArrowDiewctions.UpFinished;
        }

        if (direction == new Vector2Int(0, -1) && isFinal)
        {
            return ArrowDiewctions.DownFinished;
        }

        if (direction == new Vector2Int(1, 0) && isFinal)
        {
            return ArrowDiewctions.RightFinished;
        }

        if (direction == new Vector2Int(-1, 0) && isFinal)
        {
            return ArrowDiewctions.LeftFinished;
        }

        return ArrowDiewctions.None;
    }
}
