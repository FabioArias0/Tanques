using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // # De rondas que un jugador debe ganar para ganar el juego
    public int m_NumRoundsToWin = 5;

    // Delay entre las fases de RoundStarting y roundPlaying        
    public float m_StartDelay = 3f;         

    // Delay entre las fases RoundPalying y RoundEnding
    public float m_EndDelay = 3f;

    // Referencia al script de CameraControl           
    public CameraControl m_CameraControl;   

    // Referencia al texto para mostrar mensajes
    public Text m_MessageText;              

    // Referencia al prefab del tanque
    public GameObject m_TankPrefab;      

    // Array de TankManager para controlar cada tanque   
    public TankManager[] m_Tanks;           


    //Numero de rondas
    private int m_RoundNumber;

    //Rondas ganadas
    private int m_RoundWin;

    // Delay hasta que la ronda empieza            
    private WaitForSeconds m_StartWait;   

    // Delay hasta que la ronda acaba  
    private WaitForSeconds m_EndWait;  

    //Referencia al ganador de la ronda para anuncia quien ha ganado     
    private TankManager m_RoundWinner;

    // Referenia al ganador del juego para anunciar quien ha ganado
    private TankManager m_GameWinner;       


    private void Start()
    {
        // Creamos los delays para que solo se apliquen una vez
        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);

        //Generar tanques
        SpawnAllTanks();

        //Ajustar camara
        SetCameraTargets();

        //Iniciar juego
        StartCoroutine(GameLoop());

        //Rondas ganadas a 0
        m_RoundWin = 0;
    }


    private void SpawnAllTanks()
    {
        //Recorro los tanques
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            // Los creo, ajusto el # del jugador y las referencias
            // necesarias para controlarlo
            m_Tanks[i].m_Instance =
                Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
            m_Tanks[i].m_PlayerNumber = i + 1;
            m_Tanks[i].Setup();
        }
    }


    private void SetCameraTargets()
    {
        // Creo un array de transforms del mismo size
        // que el numero del tanque
        Transform[] targets = new Transform[m_Tanks.Length];
        //Recorro los Transforms
        for (int i = 0; i < targets.Length; i++)
        {
            // Lo ajusto al transform del tanque apropiado
            targets[i] = m_Tanks[i].m_Instance.transform;
        }

        //Estos son los targets que la camara debe de seguir
        m_CameraControl.m_Targets = targets;
    }

    // Lamado al principio y en cada fase del juego
    private IEnumerator GameLoop()
    {
        // empiezo con RoundStarting y no retorno hasta que finalice
        yield return StartCoroutine(RoundStarting());

        // Cuando finalice RoundStarting empiezo con Round Playing y no retorno
        // hasta que finalice
        yield return StartCoroutine(RoundPlaying());

        // Cuando fianalice RoundPlaying, empiezo con RoundEnding 
        // y no retorno hasta que finalice
        yield return StartCoroutine(RoundEnding());

        // Si aun no ha ganado ninguno
         if (m_GameWinner != null)
        {
            // Si hay un ganador, reinicio el nivel
            SceneManager.LoadScene(0);
        }
        else
        {
            // Sino, reinicio las co-rutinas para que continue
            // Con el bucle
            //En este caso sin yield, de modo qu esta versio del Gameloop
            //Finalizara simepre
            StartCoroutine(GameLoop());
        }
   }


    private IEnumerator RoundStarting()
    {
        // Cuando empiece la ronda reseteo los tanques e impido
        // que se muevan
        ResetAllTanks();
        DisableTankControl();
        //Ajusto la camara a los tanques reseteados
        m_CameraControl.SetStartPositionAndSize();

        //Incremento la ronda y muestro el texto informativo
        m_RoundNumber++;
        m_MessageText.text="Round"+m_RoundNumber;

        //Espero a que pase el tiempo de espera antes de volver al bucle
        yield return m_StartWait;
    }


    private IEnumerator RoundPlaying()
    {
        // Cuando empiece la ronda dejo que los tanques se muevan
        EnableTankControl();

        //Borro el texto de la pantall
        m_MessageText.text=string.Empty;

        //Miesntras haya mas de un tanque 

        while (!OneTankLeft())
        {
            // vuelvo al frame siguiente
             yield return null;

        }
       
    }


    private IEnumerator RoundEnding()
    {
        // Desabilito el movimienot de los tanques
        DisableTankControl();

        //Borro al ganador de la ronda anterior
        m_RoundWinner=null;

        //miro si hya un ganador de la ronda 
        m_RoundWinner= GetRoundWinner();

        //Si lo hay, incremento su puntuacion.

        if(m_RoundWinner!=null)
        m_RoundWinner.m_Wins++;

        //Compruebo si alguien ha ganado el juego
        m_GameWinner = GetGameWinner();

        //Genero el mensaje segun si hay un ganado del juego o no
        string message = EndMessage();
        m_MessageText.text= message;

        //Espero que pase el tiempo de espera antes de volver al bucle
        yield return m_EndWait;
    }


    //Usado para comprobar si queda mas de un tanque
    private bool OneTankLeft()
    {
        //Contador de tanques
        int numTanksLeft = 0;
        //Recorro los tanques
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            //Si esta activo, incremento el contador
            if (m_Tanks[i].m_Instance.activeSelf)
                numTanksLeft++;
        }

        //Devuelvo true si queda 1 o menos, false si queda mas de uno
        return numTanksLeft <= 1;
    }

    //Comprueba si algun tanque ha ganado la ronda 
    private TankManager GetRoundWinner()
    {
        //Recorro los tanques
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            //Si solo queda uno, es el ganador y lo devuelvo
            if (m_Tanks[i].m_Instance.activeSelf)
                return m_Tanks[i];
        }

        //Si no hay ningun activo es un empate, asi devuelvo null
        return null;
    }


    //Comprueba si hay algun ganador del juego
    private TankManager GetGameWinner()
    {
        // Recorro los tanques
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            // Si alguno tiene rondas necesarias, ha ganado
            // y lo devuelvo
            if (m_Tanks[i].m_Wins == m_NumRoundsToWin)
                return m_Tanks[i];
        }

        //Si no, devuelvo null 
        return null;
    }


    //Devuelve el texto dle mensaje a mostrar al final
    // de cada ronda
    private string EndMessage()
    {
        // Por defecto no hya ganadores, asi que es empate
        string message = "EMPATE!";

        //Si hay un ganador de ronda cambio el mensaje

        if (m_RoundWinner != null)
            message = m_RoundWinner.m_ColoredPlayerText + " GANA LA RONDA!";
        //Retornos de carro
        message += "\n\n\n\n";

        //Recorro los tanques y añado sus puntuaciones
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Tanks[i].m_Wins + " GANA\n";
            m_RoundWin = m_RoundWin + m_Tanks[i].m_Wins;
        }

        //Si hay un ganador del juego, cambio el mensaje entero para reflejarlo
        if (m_GameWinner != null)
            message = m_GameWinner.m_ColoredPlayerText + " GANA EL JUEGO!" + "\n CON " + m_NumRoundsToWin+" RONDAS GANADAS\n";
            
        return message;
    }

    //Para resetear los tanques, propiedades, posiciones...
    private void ResetAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].Reset();
        }
    }


    //Habilita el control del tanque
    private void EnableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].EnableControl();
        }
    }


//Deshabilita el control del tanque 
    private void DisableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].DisableControl();
        }
    }
}