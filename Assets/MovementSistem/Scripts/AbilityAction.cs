using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class AbilityAction : MonoBehaviour
{
    private CharacterInfo character;
    private MouseController mouseController;
    public GameObject abilityCardPrefab;
    

    public void PressBtn()
    {
        mouseController = GameObject.Find("Cursor").GetComponent<MouseController>();
        character = mouseController.character;
        if (!character.haveAttacked)
        {
            CanvasInstance.Instance.btnAcciones.SetActive(false);
            //mouseController.isFreeFocus = true;
            //mouseController.attackAction = true;
            CanvasInstance.Instance.btnQuit.SetActive(true);
            character.menu.SetActive(false);
            AbilityMenuInstance.Instance.transform.gameObject.SetActive(true);
            foreach (Transform ab in AbilityMenuInstance.Instance.container.transform)
            {
                Destroy(ab.gameObject);
            }
            foreach (Ability ab in character.habilities)
            {
                if (ab.levelToUnlock <= character.level)
                {
                    GameObject abilityCard = Instantiate(abilityCardPrefab, AbilityMenuInstance.Instance.container.transform);
                    abilityCard.transform.GetChild(1).transform.GetComponent<TextMeshProUGUI>().text = ab.abilityName;
                    abilityCard.transform.GetChild(2).transform.GetComponent<TextMeshProUGUI>().text = ab.manaCost + "MP";
                    abilityCard.transform.GetChild(1).GetComponent<AbilityShowInfo>().textInfo = ab.abilityInfoText;
                    abilityCard.GetComponent<AbilityMenuAction>().ability = ab;
                }
            }

            if (character.habilities.Count >= 6)
            {
                int sumAlt = (character.habilities.Count - 5) * 118;
                AbilityMenuInstance.Instance.scroll.transform.GetComponent<ScrollRect>().enabled = true;
                AbilityMenuInstance.Instance.container.transform.GetComponent<RectTransform>().sizeDelta =
                    new Vector2(AbilityMenuInstance.Instance.container.transform.GetComponent<RectTransform>().sizeDelta.x,
                    AbilityMenuInstance.Instance.container.transform.GetComponent<RectTransform>().sizeDelta.y + sumAlt);

            }
            else
            {
                AbilityMenuInstance.Instance.scroll.transform.GetComponent<ScrollRect>().enabled = false;
            }


        }
        else
        {
            Debug.Log("Ya has atacado");
        }
    }
}
