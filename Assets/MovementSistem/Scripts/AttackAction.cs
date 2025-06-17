using UnityEngine;

public class AttackAction : MonoBehaviour
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
            if (hit.collider.name == "attack")
            //Has clicado en el icono
            {

                if (!character.haveAttacked)
                {
                    GetInRangeTiles();
                    mouseController.isFreeFocus = true;
                    mouseController.attackAction = true;

                }
                else
                {
                    Debug.Log("Ya has atacado");
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

        mouseController.inRangeTiles = rangeFinder.GetTilesInRange(character.activeTile, character.atackRange);

        foreach (var tile in mouseController.inRangeTiles)
        {
            tile.ShowTile("attack");
        }
    }

}
