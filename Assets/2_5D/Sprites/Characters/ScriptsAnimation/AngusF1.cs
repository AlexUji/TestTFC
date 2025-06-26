using UnityEngine;

public class AngusF1 : MonoBehaviour
{
    public Animator animator;
    public GameObject angusFinalPos;
    public void EndScene2()
    {
        
        angusFinalPos.SetActive(true);
        gameObject.SetActive(false);
       
    }
}
