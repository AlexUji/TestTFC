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

    public void PressBtn()
    {
        mouseController.character.haveAttacked = true;
        mouseController.character.haveMoved = true;
        mouseController.ResetAction();
    }

    public void QuitButton()
    {
        mouseController.ResetAction();
    }
}
