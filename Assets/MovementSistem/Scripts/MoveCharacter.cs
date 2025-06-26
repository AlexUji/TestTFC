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
      
        


    }

    // Update is called once per frame

    public void PressBtn()
    {
        rangeFinder = new RangeFinder();
        
        mouseController = GameObject.Find("Cursor").GetComponent<MouseController>();
        character = mouseController.character;
        Debug.Log(character.name);

        if (!character.haveMoved)
        {
            CanvasInstance.Instance.btnQuit.SetActive(true);
            GetInRangeTiles();
            mouseController.isFreeFocus = true;
            mouseController.moveAction = true;

        }
        else
        {
            Debug.Log("Ya te has movido");
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
