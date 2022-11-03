using UnityEngine;


public class TankMovement : MonoBehaviour
{
    //Indetifica el tanque de cada jugador
    public int m_PlayerNumber = 1; 

    //Velocidad del tanque
    public float m_Speed = 12f;            
   //Velocidad que el tanque gira
    public float m_TurnSpeed = 180f;       
   //Referencia del audio
    public AudioSource m_MovementAudio;    
    //audio cuando el tanque no se mueve
    public AudioClip m_EngineIdling;       
    //Audio cuando el tanque se mueve
    public AudioClip m_EngineDriving;      
    //Afinacion del audio para que sea mas real
    public float m_PitchRange = 0.2f;

    //Nombre del eje para moverse atras y alante
    private string m_MovementAxisName;     
    //nombre del eje para girar
    private string m_TurnAxisName;         
    //Referencia para mover el tanque
    private Rigidbody m_Rigidbody;         
   //valor actual de entrada para el movimiento
    private float m_MovementInputValue;    
   //Valor actul de entrada para el giro
    private float m_TurnInputValue;        
   //Valor del pitch de 1 a fuente de audio al inico 
    private float m_OriginalPitch;         


    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }


    private void OnEnable ()
    {
        //Al habilitar el tanque, desabilitamos la kinematic del tanque 
        //para que se pueda mover
        m_Rigidbody.isKinematic = false;
        // reseteamos los valores de entrada
        m_MovementInputValue = 0f;
        m_TurnInputValue = 0f;
    }


    private void OnDisable ()
    {
        //al parar/deshabilitar el tanque, habilitamos la kinematica
        //del tanque para que se pare
        m_Rigidbody.isKinematic = true;
    }


    private void Start()
    {
        //Nombre de los ejes segun el numero de jugador
        m_MovementAxisName = "Vertical" + m_PlayerNumber;
        m_TurnAxisName = "Horizontal" + m_PlayerNumber;


        //Almaceno la afinacion original del audio del motor
        m_OriginalPitch = m_MovementAudio.pitch;
    }
    

    private void Update()
    {
        // Store the player's input and make sure the audio for the engine is playing.
        m_MovementInputValue=Input.GetAxis(m_MovementAxisName);
        m_TurnInputValue=Input.GetAxis(m_TurnAxisName);

        //Llamo la funcion que gestiona el audio del motor
        EngineAudio();
    }


    private void EngineAudio()
    {
        // Play the correct audio clip based on whether or not the tank is moving and what audio is currently playing.

        //si no hay entrada, es porque esta quieto
        if (Mathf.Abs(m_MovementInputValue)<0.1f&&Mathf.Abs(m_TurnInputValue)<0.1f)
        {
            // .... y si esta reproduciendo el audio de moverse
            if (m_MovementAudio.clip==m_EngineDriving)
            {
                // se cambia el audio al estar parado y lo reproduzco
                m_MovementAudio.clip=m_EngineIdling;
                m_MovementAudio.pitch=Random.Range(m_OriginalPitch-m_PitchRange,m_OriginalPitch+m_PitchRange);
                m_MovementAudio.Play();
            }
        }
        else
        {
            // Si hay entrada es que se esta moviendo. Si estaba resproduciendo el idle esta parado..

            if (m_MovementAudio.clip==m_EngineIdling)
            {
                // Cambio el audio al moverse y lo reproduzco
                m_MovementAudio.clip=m_EngineDriving;
                m_MovementAudio.pitch=Random.Range(m_OriginalPitch-m_PitchRange,m_OriginalPitch+m_PitchRange);
                m_MovementAudio.Play();
            }
        }
        
    }


    private void FixedUpdate()
    {
        // Move and turn the tank.
        Move();
        Turn();
    }


    private void Move()
    {
        // Adjust the position of the tank based on the player's input.
        //creo un vector en la direccion en la que apunta el tanque
        //con una magnitud basada en la entrada, velocidad y el tiempo en frames

        Vector3 movement = transform.forward*m_MovementInputValue*m_Speed*Time.deltaTime;
        //aplico ese vector de movimiento al rigidbody del tanque
        m_Rigidbody.MovePosition(m_Rigidbody.position+movement);
    }



    private void Turn()
    {
        // Adjust the rotation of the tank based on the player's input.
        //Calculo el numeor de grados de rotacion basandome en la entrada, velocidad
        // y el tiempo entre frames.
        float turn=m_TurnInputValue*m_TurnSpeed*Time.deltaTime;

        //convierto ese numeor en una rotacion en el eje Y
        Quaternion turnRotation = Quaternion.Euler(0f,turn,0f);

        //Aplico esa rotacion al rigidbody del tanque.
        m_Rigidbody.MoveRotation(m_Rigidbody.rotation*turnRotation);
    }
}