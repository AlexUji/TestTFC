using TMPro;
using UnityEngine;

public class AbilityMenuInstance : MonoBehaviour
{

    public static AbilityMenuInstance Instance;
    public TextMeshProUGUI infotext;
    public GameObject infoBox;
    public GameObject container;
    public GameObject scroll;

    void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        gameObject.SetActive(false);
    }
}
