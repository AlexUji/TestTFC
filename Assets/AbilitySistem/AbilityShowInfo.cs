using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;


[RequireComponent(typeof(TextMeshProUGUI))]
public class AbilityShowInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public string textInfo;
    

    public void OnPointerEnter(PointerEventData eventData)
    {
        
        
        AbilityMenuInstance.Instance.infoBox.SetActive(true);
        AbilityMenuInstance.Instance.infotext.text = textInfo;

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        AbilityMenuInstance.Instance.infotext.text = string.Empty;
        AbilityMenuInstance.Instance.infoBox.SetActive(false);

    }


}
