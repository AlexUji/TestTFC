using Unity.Mathematics;
using UnityEngine;

public class OverloadPunch : Ability
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        abilityName = "Overload Punch";
        sb.AppendLine("Tipo de ataque: F�sico");
        sb.AppendLine("Rango de habilidad: 1 unidad");
        sb.AppendLine("Descripci�n:");
        sb.AppendLine("Brand realiza un ataque cargado con sus pu�os y la carga de su instrumento aumentando el da�o e ignorando la mitad de la defense del enemigo");
        abilityInfoText = sb.ToString();
        manaCost = 2;
        levelToUnlock = 1;
        abilityRange = 1;
        type = UnitType.Attacker;
    }

    public override void ApplyEffect(CharacterInfo selfCharacter, CharacterInfo targetCharacter)
    {
        Debug.Log("Se ha llamado");
        //Si cumple los requisitos
        if (selfCharacter.level >= levelToUnlock && selfCharacter.currentMP >= manaCost)
        {
            
            targetCharacter.currentHP -= selfCharacter.attack*3 - math.abs((int)targetCharacter.defense / 2);
            if(targetCharacter.currentHP <= 0) { }
                selfCharacter.EnemySlayed(targetCharacter);
            base.ApplyEffect(selfCharacter, targetCharacter);
            Debug.Log("Se ha ejecutado");
        }
        else
        {
            Debug.Log("No se ha realizado la habilidad");
        }

    }
}
