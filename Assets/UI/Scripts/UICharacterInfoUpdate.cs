using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UICharacterInfoUpdate : MonoBehaviour
{
   
    public void UpdateUI(CharacterInfo targetedCharacter)
    {
        transform.GetChild(0).GetComponent<Image>().sprite = targetedCharacter.portrait;
        transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = targetedCharacter.characterName;


        //HP
        var hpinfo = transform.GetChild(2);
        hpinfo.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = targetedCharacter.currentHP.ToString();
        hpinfo.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "/" + targetedCharacter.MaxHP.ToString();

        var visulaLife = hpinfo.transform.GetChild(3).transform.GetChild(0);
        float percentageLife = (float)targetedCharacter.currentHP / (float)targetedCharacter.MaxHP;
        visulaLife.GetComponent<Image>().fillAmount = percentageLife;


        //MP
        var mpinfo = transform.GetChild(3);
        mpinfo.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = targetedCharacter.currentMP.ToString();
        mpinfo.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "/" + targetedCharacter.MaxMP.ToString();

        var visulaMana = mpinfo.transform.GetChild(3).transform.GetChild(0);
        float percentageMana = (float)targetedCharacter.currentMP / (float)targetedCharacter.MaxMP;
        visulaMana.GetComponent<Image>().fillAmount = percentageMana;

        Debug.Log("Mana " + percentageMana + ", Vida " + percentageLife);
    }
}
