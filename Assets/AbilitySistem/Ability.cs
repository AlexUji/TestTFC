using UnityEngine;

public class Ability : MonoBehaviour
{
    public string abilityName;
    //animación
    int manaCost;
    int abilityRange;
    int levelToUnlock;

    public virtual void ApplyEffect()
    {

    }
}
