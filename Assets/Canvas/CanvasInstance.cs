using UnityEngine;

public class CanvasInstance : MonoBehaviour
{
    public static CanvasInstance Instance;
    public GameObject btnQuit;
    public GameObject btnmove;
    public GameObject btnBA;
    public GameObject btnHab;
    public GameObject btnSkupt;
    public GameObject characterInfoBox;
    public GameObject btnAcciones;
    void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
       
    }
}
