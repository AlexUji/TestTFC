using UnityEngine;

public class CanvasInstance : MonoBehaviour
{
    public static CanvasInstance Instance;
    public GameObject btnQuit;
    public GameObject characterInfoBox;
    void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
       
    }
}
