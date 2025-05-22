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
    private List<OverlayTile> inRangeTiles = new List<OverlayTile>();
    private ArrowTranslator arrowtranslator;
    public GameObject currentCharacterInfoUI;

    bool isMoving = false;

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

        if (focusedTileHit.HasValue)
        {
            GameObject overlayTile = focusedTileHit.Value.collider.gameObject;
            transform.position = overlayTile.transform.position;
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = overlayTile.GetComponent<SpriteRenderer>().sortingOrder;

            if(inRangeTiles.Contains(overlayTile.GetComponent<OverlayTile>()) && !isMoving)
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
                if(character == null)
                {
                    character = Instantiate(chPre).GetComponent<CharacterInfo>();
                    PositionOnTile(overlayTile.GetComponent<OverlayTile>());
                    currentCharacterInfoUI.SetActive(true);
                    currentCharacterInfoUI.GetComponent<UICharacterInfoUpdate>().UpdateUI(character);

                    GetInRangeTiles();

                }
                else
                {
                    if (!character.isFocused)
                    {
                        character.isFocused = true;
                        character.menu.SetActive(true);

                        if (!character.haveMoved)
                        {

                            isMoving = true;
                            currentCharacterInfoUI.GetComponent<UICharacterInfoUpdate>().UpdateUI(character);

                        }
                    }
                    
                    
                }
            }

        }
        if(path.Count > 0 && isMoving)
        {
            MoveCharacterAlongPath();
            
        }
    }

    private void GetInRangeTiles()
    {
        foreach (var tile in inRangeTiles)
        {
            tile.HideTile();
        }

        inRangeTiles = rangeFinder.GetTilesInRange(character.activeTile, character.movementRange);

        foreach (var tile in inRangeTiles)
        {
            tile.ShowTile();
        }
    }

    private void MoveCharacterAlongPath()
    {
        var steep = speed * Time.deltaTime;

        var zIndex = path[0].transform.position.z;
        character.transform.position = Vector2.MoveTowards(character.transform.position, path[0].transform.position, steep);
        character.transform.position = new Vector3(character.transform.position.x, character.transform.position.y, zIndex);

        if(Vector2.Distance(character.transform.position, path[0].transform.position) < 0.0001f)
        {
            PositionOnTile(path[0]);
            path.RemoveAt(0); 
        }

        if(path.Count == 0)
        {
            GetInRangeTiles();
            isMoving = false;
            character.isFocused = false;
        }
        character.haveMoved = true;
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

    private void PositionOnTile(OverlayTile tile)
    {
        Debug.Log("Pos x: "+tile.transform.position.x + ", Pos y: " + tile.transform.position.y + ", Pos z: " + tile.transform.position.z);
        character.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, tile.transform.position.z);
        //character.GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder;
        character.activeTile = tile;
    }
}
