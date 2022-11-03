using UnityEngine;
using UnityEngine.UI;

public class TankShooting : MonoBehaviour
{
    public int m_PlayerNumber = 1;              // Se usa para identificar los jugadoes
    public Rigidbody m_Shell;                   // Prefab del la bala
    public Transform m_FireTransform;           // Hijo del tanque en donde se genera la bomba
    public Slider m_AimSlider;                  // Hijo del tanque en el cual se ve la fuerza del lanzamiento
    public AudioSource m_ShootingAudio;         // Referencia al audiosource de disparo
    public AudioClip m_ChargingClip;            // Audio de recargar
    public AudioClip m_FireClip;                // Audio cuando se dispara.
    public float m_MinLaunchForce = 15f;        // Fuerza minima del dispario si no se mantiene el boton.
    public float m_MaxLaunchForce = 30f;        // Fuerza max cuando se mantiene el boton de disparo.
    public float m_MaxChargeTime = 0.75f;       // Tiempo max de carga antes de ser lanzado el disparo con max fuerza


    private string m_FireButton;                // eje de disparo utilizado para lanzar las bombas
    private float m_CurrentLaunchForce;         // Fuerza dada la bomba cuando se suelta el boton de disparo
    private float m_ChargeSpeed;                // Velocidad de carga, basda en el max tiempo de carga.
    private bool m_Fired;                       // Bool que comprueba si se ha lanzado la bomba


    private void OnEnable()
    {
        // al crear el tanque, reseteo la fuerza de lanzamiento  y la UI
        m_CurrentLaunchForce = m_MinLaunchForce;
        m_AimSlider.value = m_MinLaunchForce;
    }


    private void Start()
    {
        // El eje de disparo basado en el # de jugador.
        m_FireButton = "Fire" + m_PlayerNumber;

        // Velocidad de carga, basada en el max tiempo de carga y los valores de carga max y min
        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
    }


    private void Update()
    {
        // asigno el valor minimo al slider
        m_AimSlider.value = m_MinLaunchForce;

        // si llego al valor max y no lo he lanzado
        if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired)
        {
            // uso el valor maximo y disparo
            m_CurrentLaunchForce = m_MaxLaunchForce;
            Fire();
        }
        // Si no, si ya he pulsado el boton de disparo
        else if (Input.GetButtonDown(m_FireButton))
        {
            // reseteo el bool de disparo y la fuerza de disparo
            m_Fired = false;
            m_CurrentLaunchForce = m_MinLaunchForce;

            // cambio el clip de audio al de cargando y lo reproduzco.
            m_ShootingAudio.clip = m_ChargingClip;
            m_ShootingAudio.Play();
        }
        // Si no, si estoy manteniendo presionado el boton disparo y aun no he disparado
        else if (Input.GetButton(m_FireButton) && !m_Fired)
        {
            // Incremento la fuerza de disparo y actualizo el slider
            m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;

            m_AimSlider.value = m_CurrentLaunchForce;
        }
        // Si ya he soltado el boton disparo y aun no he lanzado Disparo.
        else if (Input.GetButtonUp(m_FireButton) && !m_Fired)
        {
            
            Fire();
        }
    }


    private void Fire()
    {
        // ajusto el bool a true para que solo se lance una sola vez
        m_Fired = true;

        // creo una instancia de la bomba y guardo una referenca en su rigidbody
        Rigidbody shellInstance =
            Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

        // Ajusto la velocidad de la bomba en la direccion de disaparo
        shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward;

        // cambio el audio al de disparo y lo reproduzco
        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();

        // Reseteo la fuerza de lanzamiento como precaucion ante posibles eventos de boton perdidos.
        m_CurrentLaunchForce = m_MinLaunchForce;
    }
}