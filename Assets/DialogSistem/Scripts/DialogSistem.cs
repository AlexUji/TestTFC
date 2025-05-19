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

                if (UnityEngine.Random.Range(0, 2) == 0)
                    transform.GetChild(2).GetComponent<AudioSource>().resource = ndoActual.sonido1;
                else
                    transform.GetChild(2).GetComponent<AudioSource>().resource = ndoActual.sonido2;

                transform.GetChild(2).GetComponent<AudioSource>().pitch = 0.5f;
                transform.GetChild(2).GetComponent<AudioSource>().Play();
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
