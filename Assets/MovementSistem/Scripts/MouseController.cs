using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
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
    //public GameObject currentCharacterInfoUI;
    public GameObject overlayTile;
    public Ability selectedAbility;


    public bool isMoving = false;
    public bool isFreeFocus = true;
    public bool moveAction = false;
    public bool attackAction = false;
    public bool abilityAction = false;

    private void Start()
    {
        pathFinder = new PathFinder();
        rangeFinder = new RangeFinder();
        arrowtranslator = new ArrowTranslator();
        CanvasInstance.Instance.characterInfoBox.SetActive(false);


    }



    // Update is called once per frame
    void Update()
    {
        if (TurnSistem.Instance.AllyTurn)
        {
            var focusedTileHit = GetFocusOnTile();

            if (focusedTileHit.HasValue && isFreeFocus)
            {
                if (!isMoving)
                {
                    overlayTile = focusedTileHit.Value.collider.gameObject;
                    transform.position = overlayTile.transform.position;
                    gameObject.GetComponent<SpriteRenderer>().sortingOrder = 50;

                }

                if (!isMoving && inRangeTiles.Contains(overlayTile.GetComponent<OverlayTile>()))
                {
                    path = pathFinder.FindPath(character.activeTile, overlayTile.GetComponent<OverlayTile>(), inRangeTiles);

                    foreach (var tile in inRangeTiles)
                    {
                        tile.SetArrowSprite(ArrowDiewctions.None);
                    }
                    if (moveAction)
                    {
                        for (int i = 0; i < path.Count; i++)
                        {
                            var previousTile = i > 0 ? path[i - 1] : character.activeTile;
                            var futureTile = i < path.Count - 1 ? path[i + 1] : null;

                            var arrowDirection = arrowtranslator.TranslateDirections(previousTile, path[i], futureTile);
                            path[i].SetArrowSprite(arrowDirection);
                        }
                    }

                }

                //Clic en casilla
                if (Input.GetMouseButtonDown(0))
                {

                    if (attackAction)
                    {
                        if (inRangeTiles.Contains(overlayTile.GetComponent<OverlayTile>()) && overlayTile.GetComponent<OverlayTile>().characterInTile != null && overlayTile.GetComponent<OverlayTile>().characterInTile.team == Team.Enemy)
                        {
                            character.basicAttack(overlayTile.GetComponent<OverlayTile>().characterInTile.GetComponent<CharacterInfo>());
                            character.haveAttacked = true;
                            ResetAction();
                        }
                        else
                            ResetAction();
                    }
                    else if (abilityAction)
                    {
                        if (inRangeTiles.Contains(overlayTile.GetComponent<OverlayTile>()) && overlayTile.GetComponent<OverlayTile>().characterInTile != null)
                        {

                            Debug.Log("Dentro de ejecutar la acción");
                            selectedAbility.ApplyEffect(character, overlayTile.GetComponent<OverlayTile>().characterInTile);

                            character.haveAttacked = true;
                            ResetAction();
                        }
                        else
                        {
                            Debug.Log("No ha salido bien");
                            ResetAction();
                        }

                    }
                    else
                    {
                        //Si no hay character focus
                        if (character == null && overlayTile.GetComponent<OverlayTile>().characterInTile != null && overlayTile.GetComponent<OverlayTile>().characterInTile.haveActions)
                        {
                            isFreeFocus = false;
                            character = overlayTile.GetComponent<OverlayTile>().characterInTile;
                            character.isFocused = true;
                            character.menu.SetActive(true);
                            CanvasInstance.Instance.characterInfoBox.SetActive(true);
                            CanvasInstance.Instance.characterInfoBox.GetComponent<UICharacterInfoUpdate>().UpdateUI(character);
                            CanvasInstance.Instance.btnQuit.SetActive(true);

                        }
                        //Si hay character focus y se ha clicado a mover
                        else if (moveAction && inRangeTiles.Contains(overlayTile.GetComponent<OverlayTile>()) && overlayTile.GetComponent<OverlayTile>().characterInTile == null)
                        {

                            transform.position = overlayTile.transform.position;
                            gameObject.GetComponent<SpriteRenderer>().sortingOrder = 50;

                            character.activeTile.characterInTile = null;
                            overlayTile.GetComponent<OverlayTile>().characterInTile = character;
                            foreach (var tile in inRangeTiles)
                            {
                                tile.SetArrowSprite(ArrowDiewctions.None);
                                tile.HideTile();
                            }

                            isMoving = true;
                            character.menu.SetActive(false);


                        }
                    }


                }


                //Para recorrer todo el path
                if (path.Count > 0 && isMoving)
                {
                    MoveCharacterAlongPath();

                }

            }
        }
        
    }

    public void ResetAction() {

        AbilityMenuInstance.Instance.gameObject.SetActive(false);
        CanvasInstance.Instance.btnQuit.SetActive(false);
        isFreeFocus = true;
        character.isFocused = false;
        character.menu.SetActive(false);
        CanvasInstance.Instance.characterInfoBox.SetActive(false);
        CanvasInstance.Instance.characterInfoBox.GetComponent<UICharacterInfoUpdate>().UpdateUI(character);
        isMoving = false;
        moveAction = false;
        attackAction = false;
        abilityAction = false;

        foreach (var tile in inRangeTiles)
        {
            tile.SetArrowSprite(ArrowDiewctions.None);
            tile.HideTile();
        }
            inRangeTiles = new List<OverlayTile>();

        TurnSistem.Instance.EndAction(character);
        character = null;

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
            character.haveMoved = true;

            if (character.haveAttacked)
            {
                ResetAction();
            }
            else
            {
                isFreeFocus = false;
                inRangeTiles = new List<OverlayTile>();
                isMoving = false;

                moveAction = false;
                character.menu.SetActive(true);
            }

        }


    }
}
