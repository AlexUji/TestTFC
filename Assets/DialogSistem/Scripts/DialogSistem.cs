using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DialogSistem : MonoBehaviour
{
    public NodoDialogo ndoActual;
    public float velocidadTexto;

    private Sprite imgPortrait;
    private string[] lineas;
  

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine("WriteLine");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator WriteLine()
    {
        transform.GetChild(0).GetComponent<Image>().sprite = ndoActual.portrait;
        TextMeshProUGUI texto = transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        lineas = ndoActual.lineas;

        foreach (string line in lineas)
        {
            texto.text = string.Empty;
            foreach(char c in line)
            {
                texto.text += c;
                //Añadir sonido
                yield return new WaitForSeconds(velocidadTexto);
            }
              
            yield return new WaitUntil(() => Input.anyKeyDown);
        }
        if(ndoActual.nextDialog == null)
        {
            gameObject.SetActive(false);
            StopCoroutine("WriteLine");
        }
            

        else
        {
            ndoActual = ndoActual.nextDialog;
            StartCoroutine("WriteLine");
        }

    }
}
