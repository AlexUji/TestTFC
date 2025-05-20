using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DialogSistem : MonoBehaviour
{
    public NodoDialogo ndoActual;
    public float velocidadTexto;

    private string[] lineas;
    private float currentVelocidadTexto;
   

    void Start()
    {
        StartCoroutine("WriteLine");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            currentVelocidadTexto = velocidadTexto * 0.2f;

    }

    IEnumerator WriteLine()
    {
        if(ndoActual != null)
        {
            transform.GetChild(0).GetComponent<Image>().sprite = ndoActual.portrait;
            TextMeshProUGUI texto = transform.GetChild(1).GetComponent<TextMeshProUGUI>();

            lineas = ndoActual.lineas;

            foreach (string line in lineas)
            {
                transform.GetChild(3).gameObject.SetActive(false);
                texto.text = string.Empty;
                foreach (char c in line)
                {
                    texto.text += c;

                    if (UnityEngine.Random.Range(0, 2) == 0)
                        transform.GetChild(2).GetComponent<AudioSource>().resource = ndoActual.sonido1;
                    else
                        transform.GetChild(2).GetComponent<AudioSource>().resource = ndoActual.sonido2;

                    transform.GetChild(2).GetComponent<AudioSource>().pitch = 0.5f;
                    transform.GetChild(2).GetComponent<AudioSource>().Play();
                    yield return new WaitForSeconds(currentVelocidadTexto);
                }
                currentVelocidadTexto = velocidadTexto;
                transform.GetChild(3).gameObject.SetActive(true);
                yield return new WaitUntil(() => Input.anyKeyDown);

            }

            ndoActual = ndoActual.nextDialog;
            StartCoroutine("WriteLine");
        }
        else if (Input.anyKeyDown)
        {
            transform.GetChild(3).gameObject.SetActive(false);
            gameObject.SetActive(false);
            StopCoroutine("WriteLine");
        }
    }
}
