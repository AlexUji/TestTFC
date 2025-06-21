using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static ArrowTranslator;

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
                mouseController.character.haveAttacked = true;
                mouseController.character.haveMoved = true;
                mouseController.ResetAction();

            }
        }

    }

    public void QuitButton()
    {
        mouseController.ResetAction();
    }
}
