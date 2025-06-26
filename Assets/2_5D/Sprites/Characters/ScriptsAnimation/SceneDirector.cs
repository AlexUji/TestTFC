using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDirector : MonoBehaviour
{
    public static SceneDirector Instance;
    public Animator brant;
    public Animator angus;
    public Animator Director;
    public GameObject dBox;
    public int escena = 2;
    public NodoDialogo escena2;
    public NodoDialogo escena3;
    void Awake()
    {
        Instance = this;
    }

    public void NextScene()
    {
        
        if (escena == 1)
        {
           
            Director.SetBool("Escena1", true);
            Director.SetBool("Stop", false);


        }
        else if(escena == 2)
        {
            Debug.Log("Escena");
            Director.SetBool("Escena1", false);
            Director.SetBool("Escena2", true);

        }
        else if (escena == 3) 
        {
            SceneManager.LoadScene(1);

        }
        escena++;
    }
    public void StopDirec()
    {
        Debug.Log("Stop");
        Director.SetBool("Stop", true);

    }
    public void BrantIdle()
    {
        brant.SetBool("IsIdle", true);
    }

    public void AngusIdle()
    {
        angus.SetBool("MoveAngus", false);
    }

    public void MoveAngus()
    {
        angus.SetBool("MoveAngus", true);
    }
    public void InitDialog1()
    {
        brant.SetBool("IsIdle", false);
        brant.SetBool("Escena1", true);
        
    }

    public void InitDialog2() {
        
        Director.SetBool("Stop", false);
        angus.SetBool("MoveAngus", true);
        brant.SetBool("IsIdle", false);
        brant.SetBool("Escena2", true);
    }

    public void InitDialog3()
    {
        dBox.SetActive(true);
        dBox.GetComponent<DialogSistem>().ndoActual = escena3;
        dBox.GetComponent<DialogSistem>().StartDialog();
    }

}
