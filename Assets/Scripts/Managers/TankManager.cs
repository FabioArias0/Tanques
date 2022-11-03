using System;
using UnityEngine;

//Hace que los atributos aparezcan ene l inspector si no lo escondemos
[Serializable]
public class TankManager
{
    // Esta clase gestiona la configuracion del tanque junto
    // con el GameManger. Gestiona el comportamiento de los tanques
    // y si los jugadores tienen control sobre el tanque.
    // en los distintos momentos del juego

    // Color para el tanque
    public Color m_PlayerColor;
    // posicion y direccion en la que se generara el tanque            
    public Transform m_SpawnPoint;
    //Especifica con que jugador esta actuando el game manager         
    [HideInInspector] public int m_PlayerNumber;          

    // String que representa el color del tanque   
    [HideInInspector] public string m_ColoredPlayerText;

    // Referencia a la instancia dle tanque cuando se crea
    [HideInInspector] public GameObject m_Instance;       

    // numero de victorias del jugador   
    [HideInInspector] public int m_Wins;    



    // Referencia al script de movimiento del tanque,utilizado 
    // para dehabilitar y habilitar el control.
    private TankMovement m_Movement;       

    // Referencia al script de dispario del tanque. Utilizado para
    // deshabilitar y habilitar el control
    private TankShooting m_Shooting;

    //Utilizado para deshabilitar el UI del munod durante las fases
    // del inicio y fin de cada ronda
    private GameObject m_CanvasGameObject;


    public void Setup()
    {
        //Tomo referencia de los componentes
        m_Movement = m_Instance.GetComponent<TankMovement>();
        m_Shooting = m_Instance.GetComponent<TankShooting>();
        m_CanvasGameObject = m_Instance.GetComponentInChildren<Canvas>().gameObject;

        //Ajusto los # de jugadores para que sean iguales en todos
        // los scripts
        m_Movement.m_PlayerNumber = m_PlayerNumber;
        m_Shooting.m_PlayerNumber = m_PlayerNumber;

        //Creo un string usanod el color del tanque que diga PLayer1
        m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">PLAYER " + m_PlayerNumber + "</color>";

        //Tomo todos los renders del tanque
        MeshRenderer[] renderers = m_Instance.GetComponentsInChildren<MeshRenderer>();

        //los recorro
        for (int i = 0; i < renderers.Length; i++)
        {
            // Y ajusto el color del material del tanque
            renderers[i].material.color = m_PlayerColor;
        }
    }

    //Usado durante las fases del juego en las que el jugador no 
    // debe poder controlar el tanque.
    public void DisableControl()
    {
        m_Movement.enabled = false;
        m_Shooting.enabled = false;

        m_CanvasGameObject.SetActive(false);
    }


    // Usando durante las fases del juego en las que el jugador 
    //debe poder controlar el tanque
    public void EnableControl()
    {
        m_Movement.enabled = true;
        m_Shooting.enabled = true;

        m_CanvasGameObject.SetActive(true);
    }


    //Usado al inicio de cada ronda para poner el tanque en su
    //estado inicial.
    public void Reset()
    {
        m_Instance.transform.position = m_SpawnPoint.position;
        m_Instance.transform.rotation = m_SpawnPoint.rotation;

        m_Instance.SetActive(false);
        m_Instance.SetActive(true);
    }
}
