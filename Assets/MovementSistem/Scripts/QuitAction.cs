using UnityEngine;
using UnityEngine.TextCore.Text;

public class QuitAction : MonoBehaviour
{

    private MouseController mouseController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mouseController = GameObject.Find("Cursor").GetComponent<MouseController>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
           
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider.name == "quit")
            {
                mouseController.isFreeFocus = true;
                mouseController.character.isFocused = false;
                mouseController.character.menu.SetActive(false);
                mouseController.currentCharacterInfoUI.SetActive(false);
                mouseController.character = null;

            }
        }

    }
}
