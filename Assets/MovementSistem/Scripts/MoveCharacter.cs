using System.IO;
using UnityEngine;
using static ArrowTranslator;
using UnityEngine.TextCore.Text;
using System.Collections.Generic;

public class MoveCharacter : MonoBehaviour
{
    private RangeFinder rangeFinder;

    private CharacterInfo character;
    private MouseController mouseController;
    private void Start()
    {
      
        rangeFinder = new RangeFinder();
        character = transform.GetComponentInParent<CharacterInfo>();
        mouseController = GameObject.Find("Cursor").GetComponent<MouseController>();


    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if (hit.collider.name == "move")
            //Has clicado en el icono
            {
               
                if (!character.haveMoved)
                {
                    CanvasInstance.Instance.btnQuit.SetActive(true);
                    GetInRangeTiles();
                    mouseController.isFreeFocus = true;
                    mouseController.moveAction = true;

                }
                else {
                    Debug.Log("Ya te has movido");
                }
               
              
            }

            ////
        }
    }

    private void GetInRangeTiles()
    {
        foreach (var tile in mouseController.inRangeTiles)
        {
            tile.HideTile();
        }

        mouseController.inRangeTiles = rangeFinder.GetTilesInRange(character.activeTile, character.movementRange);

        foreach (var tile in mouseController.inRangeTiles)
        {
            tile.ShowTile(UnitType.move);
        }
    }

}
