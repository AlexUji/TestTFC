using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ArrowTranslator;

public class MouseController : MonoBehaviour
{
    public float speed;
    public GameObject chPre;
    public CharacterInfo character;

    private PathFinder pathFinder;
    private RangeFinder rangeFinder;
    private List<OverlayTile> path = new List<OverlayTile>();
    public List<OverlayTile> inRangeTiles = new List<OverlayTile>();
    private ArrowTranslator arrowtranslator;
    public GameObject currentCharacterInfoUI;
    public GameObject overlayTile;

    public bool isMoving = false;
    public bool isFreeFocus = true;
    public bool moveAction = false;

    private void Start()
    {
        pathFinder = new PathFinder();
        rangeFinder = new RangeFinder();
        arrowtranslator = new ArrowTranslator();
        currentCharacterInfoUI = GameObject.Find("UIInfoCharacterTargeted");
        currentCharacterInfoUI.SetActive(false);

    }



    // Update is called once per frame
    void Update()
    {
        var focusedTileHit = GetFocusOnTile();

        if (focusedTileHit.HasValue && isFreeFocus)
        {
            overlayTile = focusedTileHit.Value.collider.gameObject;
            transform.position = overlayTile.transform.position;
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = 100;

            if(!isMoving && inRangeTiles.Contains(overlayTile.GetComponent<OverlayTile>()))
            {
                path = pathFinder.FindPath(character.activeTile, overlayTile.GetComponent<OverlayTile>(), inRangeTiles);

                foreach (var tile in inRangeTiles)
                {
                    tile.SetArrowSprite(ArrowDiewctions.None);
                }

                for (int i = 0; i < path.Count; i++)
                {
                    var previousTile = i > 0 ? path[i - 1] : character.activeTile;
                    var futureTile = i < path.Count - 1 ? path[i + 1] : null;

                    var arrowDirection = arrowtranslator.TranslateDirections(previousTile, path[i], futureTile);
                    path[i].SetArrowSprite(arrowDirection);
                }
            }

            //Clic en casilla
           if (Input.GetMouseButtonDown(0))
            {
                //Si no hay character focus
                if(overlayTile.GetComponent<OverlayTile>().characterInTile != null)
                {
                    isFreeFocus = false;
                    character = overlayTile.GetComponent<OverlayTile>().characterInTile;
                    character.isFocused = true;
                    character.menu.SetActive(true);
                    currentCharacterInfoUI.SetActive(true);
                    currentCharacterInfoUI.GetComponent<UICharacterInfoUpdate>().UpdateUI(character);

                }
                //Si hay character focus y se ha clicado a mover
                else if (moveAction)
                {
                    foreach (var tile in inRangeTiles)
                    {
                        tile.SetArrowSprite(ArrowDiewctions.None);
                        tile.HideTile();
                    }

                    isMoving = true;
                    character.menu.SetActive(false);
                   
                }

            }


           //Para recorrer todo el path
            if (path.Count > 0 && isMoving)
            {
                MoveCharacterAlongPath();

            }

        }
        
    }



    public RaycastHit2D? GetFocusOnTile()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePosition2D = new Vector2(mousePosition.x, mousePosition.y);

        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePosition2D, Vector2.zero);

        if (hits.Length > 0)
        {
            return hits.OrderByDescending(i => i.collider.transform.position.z).First();
        }

        return null;
    }

    public void PositionOnTile(OverlayTile tile)
    {
        Debug.Log("Pos x: "+tile.transform.position.x + ", Pos y: " + tile.transform.position.y + ", Pos z: " + tile.transform.position.z);
        character.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, tile.transform.position.z);
        //character.GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder;
        character.activeTile = tile;
    }
    private void MoveCharacterAlongPath()
    {
        var steep = speed * Time.deltaTime;

        var zIndex = path[0].transform.position.z;
        character.transform.position = Vector2.MoveTowards(character.transform.position, path[0].transform.position, steep);
        character.transform.position = new Vector3(character.transform.position.x, character.transform.position.y, zIndex);

        if (Vector2.Distance(character.transform.position, path[0].transform.position) < 0.0001f)
        {
            PositionOnTile(path[0]);
            path.RemoveAt(0);
        }

        if (path.Count == 0)
        {

            inRangeTiles = new List<OverlayTile>();
            isMoving = false;
            character.haveMoved = true;
            isFreeFocus = true;
            moveAction = false;
            character.menu.SetActive(true);
            //character.isFocused = false;
            //character.GetComponent<SpriteRenderer>().color = new Color(0.75f, 0.75f, 0.75f, 1);

        }


    }
}
