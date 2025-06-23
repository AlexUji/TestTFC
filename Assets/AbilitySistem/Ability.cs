using System.Text;
using UnityEngine;

public class Ability : MonoBehaviour
{
    public string abilityName;
    public StringBuilder sb = new StringBuilder();
    public string abilityInfoText;
    public UnitType type;
    //animación
    public int manaCost;
    public int abilityRange;
    public int levelToUnlock;
    

    public virtual void ApplyEffect(CharacterInfo selfCharacter, CharacterInfo targetCharacter)
    {
        selfCharacter.currentMP -= manaCost;
        selfCharacter.haveAttacked = true;
    }

    public virtual int SimulateAttack(CharacterInfo selfCharacter, CharacterInfo targetCharacter)
    {
        return 0;
    }

    public virtual int SimulateHeal(CharacterInfo selfCharacter, CharacterInfo targetCharacter)
    {
        return 0;
    }

    public virtual int SimulateDebuff(CharacterInfo selfCharacter, CharacterInfo targetCharacter)
    {
        return 0;
    }

    public virtual int SimulateBuff(CharacterInfo selfCharacter, CharacterInfo targetCharacter)
    {
        return 0;
    }
}
