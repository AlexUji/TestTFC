using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;


[RequireComponent(typeof(TextMeshProUGUI))]
public class AbilityShowInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{


    public GameObject abilityInfoBox;

    public void OnPointerEnter(PointerEventData eventData)
    {
     //   isHovering = true;
        Debug.Log("El ratón está sobre el texto TMP.");

        abilityInfoBox.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = transform.GetComponent<TextMeshProUGUI>().text+" Texto de prueba funciona de locos";
        abilityInfoBox.SetActive(true);



    }

    public void OnPointerExit(PointerEventData eventData)
    {
      //  isHovering = false;
        Debug.Log("El ratón ha salido del texto TMP.");

        abilityInfoBox.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = string.Empty;
        abilityInfoBox.SetActive(false);
    }


    /*
    // Update is called once per frame
    void Update()
    {


        if (isHovering)
        {
            Debug.Log("MostrarTextoInfo");
        }


    }*/
}
