using UnityEngine;

public class Ability : MonoBehaviour
{
    public string abilityName;
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
