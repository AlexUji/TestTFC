using UnityEngine;

public class golpe : MonoBehaviour
{

    public void DesactivarGO()
    {
        if(transform.parent.GetComponent<CharacterInfo>().currentHP <= 0)
        {
            Destroy(transform.parent.gameObject);
        }
        gameObject.SetActive(false);
    }

}
