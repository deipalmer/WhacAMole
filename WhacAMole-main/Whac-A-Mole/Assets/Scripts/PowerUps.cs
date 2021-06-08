using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUps : MonoBehaviour
{
    public float numero = 200;
    Vector3 posicionInicial;
    int probabilidadBomba = 50;
    int probabilidadTiempoLento = 20;
    public GameObject bomba;
    public GameObject tiempoLento;
    public Vector3 pos;
    bool haSalido = false;
    public bool timeRunning = false;
    // Start is called before the first frame update
    void Awake()
    {
        posicionInicial = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //verificamos que el juego ha empezado
        if (timeRunning == true)
        {
            //calculamos probabilidades
            CalcularProb();
            //Aparecen los objetos si se cumplen los requisitos
            AparecerBomba();
            AparecerTiempoLento();
        }
    }

    public void CalcularProb()
    {
        numero = Random.Range(0, 100);
    }

    public void TimeRunning()
    {
        timeRunning = !timeRunning;
    }

    public void AparecerBomba()
    {
        float tiempo = Random.Range(2f, 4f);
        if ((numero > probabilidadBomba) && (haSalido == false))
        {
            LeanTween.move(bomba, pos, tiempo);
            haSalido = true;
            numero = 0;
        }
    }

    public void AparecerTiempoLento()
    {
        float tiempo = Random.Range(2f, 4f);
        if ((numero < probabilidadTiempoLento) && (haSalido == false))
        {
            LeanTween.move(tiempoLento, pos, tiempo);
            haSalido = true;
            numero = 0;
        }
    }

    public void HitBomb()
    {
        LeanTween.scale(bomba,new Vector3(0, 0, 0) , 0.2f);
    }

    public void HitTiempoLento()
    {
        LeanTween.scale(bomba, new Vector3(0, 0, 0), 0.2f);
    }
}
