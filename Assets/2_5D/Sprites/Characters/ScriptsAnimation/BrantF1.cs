using UnityEngine;

public class BrantF1 : MonoBehaviour
{
    public GameObject dbox;
    public GameObject finalPos;
    public Animator animator;
 
   
   public void EndScene1()
    {
        animator.SetBool("Escena1", false);
        animator.SetBool("IsIdle", true);
        dbox.SetActive(true);
        dbox.GetComponent<DialogSistem>().StartDialog();
    }

    public void EndScene2()
    {
        finalPos.SetActive(true);
        
        dbox.SetActive(true);
        dbox.GetComponent<DialogSistem>().ndoActual = SceneDirector.Instance.escena2;
        dbox.GetComponent<DialogSistem>().StartDialog();
        gameObject.SetActive(false);

    }

}
