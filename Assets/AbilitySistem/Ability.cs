using System.Text;
using UnityEngine;

public class Ability : MonoBehaviour
{
    public string abilityName;
    public StringBuilder sb = new StringBuilder();
    public string abilityInfoText;
    //animación
    public int manaCost;
    public int abilityRange;
    public int levelToUnlock;
    

    public virtual void ApplyEffect(CharacterInfo selfCharacter, CharacterInfo targetCharacter)
    {
        selfCharacter.currentMP -= manaCost;
        selfCharacter.haveAttacked = true;
    }
}
