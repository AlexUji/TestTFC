using UnityEngine;

public class AttackAction : MonoBehaviour
{
    private RangeFinder rangeFinder;

    private CharacterInfo character;
    private MouseController mouseController;


    public void PressBtn()
    {
        rangeFinder = new RangeFinder();

        mouseController = GameObject.Find("Cursor").GetComponent<MouseController>();
        character = mouseController.character;

        if (!character.haveAttacked)
        {
            character.menu.SetActive(false);
            CanvasInstance.Instance.btnQuit.SetActive(true);
            GetInRangeTiles();
            mouseController.isFreeFocus = true;
            mouseController.attackAction = true;

        }
        else
        {
            Debug.Log("Ya has atacado");
        }
    }

    private void GetInRangeTiles()
    {
        foreach (var tile in mouseController.inRangeTiles)
        {
            tile.HideTile();
        }

        mouseController.inRangeTiles = rangeFinder.GetTilesInRange(character.activeTile, character.atackRange);

        foreach (var tile in mouseController.inRangeTiles)
        {
            tile.ShowTile(UnitType.Attacker);
        }
    }

}
