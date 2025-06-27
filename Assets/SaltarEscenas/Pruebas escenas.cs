using UnityEngine;
using UnityEngine.SceneManagement;

public class Pruebasescenas : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void IrAEscena1()
    {
        SceneManager.LoadScene(2);
       
    }

    public void IrAEscena2()
    {
        SceneManager.LoadScene(1);
    }
}
