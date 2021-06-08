using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public GameObject mainMenu, inGameUI, endScreen, recordPanel;

    public Transform molesParent;
    private MoleBehaviour[] moles;

    public bool playing = false;

    public float gameDuration = 60f;
    public float timePlayed;
    public float tiempoPowerUp = 5f;
    bool powerUpCogido = false;
    public TextMeshProUGUI timePlayedText;

    public int points = 0;
    public TextMeshProUGUI pointsText;
    public int record = 0;
    public int clicks = 0;
    public int clickAcierto = 0;
    public float porcentajeAcierto;
    public int failedClicks = 0;

    public Transform powerUpParent;
    PowerUps[] powerUps;

    public TMP_InputField nameField;
    string playerName;

    public TextMeshProUGUI infoGame;

    void Awake()
    {
        if (GameController.instance == null)
        {
            ConfigureInstance();
        }
        else
        {
            Destroy(this);
        }

    }

    void ConfigureInstance()
    {
        //Configura acceso a moles
        moles = new MoleBehaviour[molesParent.childCount];
        for (int i = 0; i < molesParent.childCount; i++)
        {
            moles[i] = molesParent.GetChild(i).GetComponent<MoleBehaviour>();
        }

        powerUps = new PowerUps[powerUpParent.childCount];
        for (int i = 0; i < powerUpParent.childCount; i++)
        {
            powerUps[i] = powerUpParent.GetChild(i).GetComponent<PowerUps>();
        }

        //Inicia los puntos
        points = 0;
        clicks = 0;
        porcentajeAcierto = 0;
        clickAcierto = 0;
        failedClicks = 0;

        //Activa la UI inicial
        inGameUI.SetActive(false);
        mainMenu.SetActive(true);
        endScreen.SetActive(false);
        recordPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //Aqui veremos si el power Up ha sido cogido o no, y haremos una cuenta atras con su duracion
        if (playing == true)
        {
            if (powerUpCogido == true)
            {
                tiempoPowerUp -= Time.deltaTime;

                timePlayed += Time.deltaTime / 3;

                if (tiempoPowerUp <= 0)
                {
                    tiempoPowerUp = 0;
                    timePlayed += Time.deltaTime;
                }
            }
            else
            {
                timePlayed += Time.deltaTime;
            }

            if (timePlayed >= gameDuration)
            {

                ShowEndScreen();
                playing = false;
                for (int i = 0; i < moles.Length; i++)
                {
                    moles[i].StopMole();
                }
            }
            else
            {
                
                CheckClicks();
                //Aqui mostraremos los puntos en pantalla
                pointsText.text = MostrarPuntos(points);
                //Aqui mostraremos el tiempo en pantalla
                timePlayedText.text = MostarTiempo(timePlayed);
            }
            
        }

        
    }


    void ShowEndScreen()
    {
        //Calculamos el porcentaje de aciertos al finalizar la partida
        porcentajeAcierto = ((float)clickAcierto / (float)clicks) * 100;
        endScreen.SetActive(true);

        bool isRecord = false;
        
        //Comprobamos si el record es mayor que el anterior, y si lo es lo sustituimos
        if (points > record)
        {
            record = points;
            isRecord = true;
        }

        else
        {
            isRecord = false;
        }

        infoGame.text = " Total points : " + points.ToString("0000") + "\n Record: " + record.ToString("0000") + "\n"+ porcentajeAcierto.ToString() + " % good shots \n" + failedClicks.ToString("") + " bad shots";
        //si hay nuevo record mostrar el panel recordPanel
        recordPanel.SetActive(isRecord);
    }

    /// <summary>
    /// Function called from End Screen when players hits Retry button
    /// </summary>
    public void Retry()
    {
        //Guardar record si es necesario

        //Acceso al texto escrito
        playerName = nameField.text;
        Debug.Log("Record de " + playerName);

        //Reinicia información del juego
        ResetGame();
        //Cambia las pantallas
        inGameUI.SetActive(true);
        mainMenu.SetActive(false);
        endScreen.SetActive(false);
        //Activa juego
        playing = true;

        //Reinicia moles
        for (int i = 0; i < moles.Length; i++)
        {
            moles[i].ResetMole();
        }
    }

    /// <summary>
    /// Restarts all info game
    /// </summary>
    void ResetGame()
    {
        for (int i = 0; i < moles.Length; i++)
        {
            moles[i].StopMole();
        }

        timePlayed = 0.0f;
        points = 0;
        clicks = 0;
        clickAcierto = 0;
        failedClicks = 0;
    }

    public void EnterMainScreen()
    {
        playerName = nameField.text;
        Debug.Log("Record de " + playerName);
        //Reinicia información del juego
        ResetGame();
        //Cambia las pantallas
        inGameUI.SetActive(false);
        mainMenu.SetActive(true);
        endScreen.SetActive(false);
        recordPanel.SetActive(false);

    }

    /// <summary>
    /// Used to check if players hits or not the moles/powerups
    /// </summary>
    public void CheckClicks()
    {
        if ((Input.touchCount >= 1 && Input.GetTouch(0).phase == TouchPhase.Ended) || (Input.GetMouseButtonUp(0)))
        {
          
            Vector3 pos = Input.mousePosition;
            if (Application.platform == RuntimePlatform.Android)
            {
                pos = Input.GetTouch(0).position;
            }

            Ray rayo = Camera.main.ScreenPointToRay(pos);
            RaycastHit hitInfo;
            if (Physics.Raycast(rayo, out hitInfo))
            {
                clicks++;

                if (hitInfo.collider.tag.Equals("Mole"))
                {
                    MoleBehaviour mole = hitInfo.collider.GetComponent<MoleBehaviour>();
                    if (mole != null)
                    {
                        mole.OnHitMole();
                        //Le damos a los topos un valor en su script y los sumamos aqui
                        points += mole.puntosQueDa;
                        //y añadimos un acierto si se le da a un objeto con el tag Mole
                        clickAcierto++;
                    }
                }

                else if (hitInfo.collider.tag.Equals("Bomba"))
                {
                    PowerUps bomb = hitInfo.collider.GetComponent<PowerUps>();
                    if (bomb != null)
                    {
                        //Hacemos desaparecer el power Up
                        bomb.HitBomb();
                        //y usamos la funcion OnHitMole junto a un for para que detecte a los topos que estan visibles
                        for (int i = 0; i < moles.Length; i++)
                        {
                            moles[i].OnHitMole();
                            points = moles[i].puntosQueDa;
                            clickAcierto++;
                        }
                    }
                }

                else if (hitInfo.collider.tag.Equals("TiempoLento"))
                {
                    PowerUps tiempoLento = hitInfo.collider.GetComponent<PowerUps>();
                    if (tiempoLento != null)
                    {
                        //Aqui vemos si el power up ha sido cogido
                        powerUpCogido = true;
                        //desaparecemos el power up
                        tiempoLento.HitTiempoLento();
                        //y añadimos un acierto
                        clickAcierto++;
                    }
                }
                //si no se le consigue dar a ninguno de los anteriores objetos contara como un fallo
                else
                {
                    failedClicks++;
                }
            }
        }
    }

    public void OnGameStart()
    {
        mainMenu.SetActive(false);
        inGameUI.SetActive(true);

        for (int i = 0; i < moles.Length; i++)
        {
            moles[i].ResetMole(moles[i].initTimeMin, moles[i].initTimeMax);
        }

        for (int i = 0; i < powerUps.Length; i++)
        {
            powerUps[i].TimeRunning();
        }

        playing = true;
        clicks = -1;
        porcentajeAcierto = 0;
        failedClicks = -1;
        points = 0;
    }
    /// <summary>
    /// Funcion para entrar en pausa, pone playing en false y muestra la pantalla de pausa.
    /// </summary>
    public void EnterOnPause()
    { 
    
    
    }

    public string MostrarPuntos(int puntuacion)
    {
        return puntuacion.ToString("0000");
    }

    public string MostarTiempo(float tiempo)
    {
        int minutos = (int)tiempo / 60;
        int segundos = (int)tiempo % 60;

        return minutos.ToString("00") + (":") + segundos.ToString("00");
    }
}
