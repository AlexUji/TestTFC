using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static ArrowTranslator;

public class OverlayTile : MonoBehaviour
{
    public int G;
    public int H;

    public int F { get { return G + H; } }

    public bool isBlocked;
    public OverlayTile previusTile;
    public Vector3Int gridPosition;
    public Vector2Int grid2DPosition { get {return new Vector2Int(gridPosition.x, gridPosition.y); } }

    public List<Sprite> arrows;


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //HideTile();
        }

    }
    public void ShowTile()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
    }

    public void HideTile()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        SetArrowSprite(ArrowDiewctions.None);
    }

    public void SetArrowSprite(ArrowDiewctions direction )
    {
        var arrow = GetComponentsInChildren<SpriteRenderer>()[1];
        if(direction == ArrowDiewctions.None)
        {
            arrow.color = new Color(1, 1, 1, 0);
        }
        else
        {

            arrow.color = new Color(1, 1, 1, 1);
            arrow.sprite = arrows[(int)direction];
            //arrow.sortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
        }
    }
}
