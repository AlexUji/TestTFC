using UnityEngine;

public class Ability : MonoBehaviour
{
    public string abilityName;
    //animaci�n
    int manaCost;
    int abilityRange;
    int levelToUnlock;

    public virtual void ApplyEffect()
    {

    }
}
