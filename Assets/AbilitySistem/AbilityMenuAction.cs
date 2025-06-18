using UnityEngine;
using UnityEngine.TextCore.Text;

public class AbilityMenuAction : MonoBehaviour
{
    private RangeFinder rangeFinder;

    private CharacterInfo character;
    private MouseController mouseController;
    public Ability ability;
    private void Start()
    {

        rangeFinder = new RangeFinder();
        
        mouseController = GameObject.Find("Cursor").GetComponent<MouseController>();
        character = mouseController.character;

    }

    public void PressButton()
    {
            GetInRangeTiles();
            mouseController.isFreeFocus = true;
            mouseController.abilityAction = true;
            mouseController.selectedAbility = ability;
            AbilityMenuInstance.Instance.gameObject.SetActive(false);
    }

    private void GetInRangeTiles()
    {
        foreach (var tile in mouseController.inRangeTiles)
        {
            tile.HideTile();
        }
        mouseController.inRangeTiles = rangeFinder.GetTilesInRange(character.activeTile, ability.abilityRange);

        foreach (var tile in mouseController.inRangeTiles)
        {
            tile.ShowTile("attack");
        }
    }
}
