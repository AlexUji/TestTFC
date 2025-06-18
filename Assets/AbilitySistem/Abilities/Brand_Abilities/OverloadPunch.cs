using Unity.Mathematics;
using UnityEngine;

public class OverloadPunch : Ability
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        abilityName = "Overload Punch";
        manaCost = 2;
        levelToUnlock = 1;
        abilityRange = 1;
    }

    public override void ApplyEffect(CharacterInfo selfCharacter, CharacterInfo targetCharacter)
    {
        //Si cumple los requisitos
        if (selfCharacter.level >= levelToUnlock && selfCharacter.currentMP >= manaCost)
        {
            targetCharacter.currentHP -= selfCharacter.attack*3 - math.abs((int)targetCharacter.defense / 2);
            base.ApplyEffect(selfCharacter, targetCharacter);
        }

    }
}
