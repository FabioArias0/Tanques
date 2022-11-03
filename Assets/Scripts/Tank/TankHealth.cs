using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour
{
    // Cantidad de salud que empieza el tanque
    public float m_StartingHealth = 100f;  
    //Slider que representa la salud del tanque        
    public Slider m_Slider;   
    //Componente de imagen del slider                     
    public Image m_FillImage;     
    //Color del slider con salud completa                 
    public Color m_FullHealthColor = Color.green;  
    //Colode de slider con salud vacia
    public Color m_ZeroHealthColor = Color.red; 
    //Prefab que intanciamos al inicio y usamos cuando el tanque se muere
       
    public GameObject m_ExplosionPrefab;
    
    //Fuente de audio al reproducir cuando el tanque explota
    private AudioSource m_ExplosionAudio;     
    //Sistemas de particulas que se reproducen cuando el tanque se destruye     
    private ParticleSystem m_ExplosionParticles;
    //Variable para almacenar la salud del tanque   
    private float m_CurrentHealth;  

    //Variable para comprobar si el tanque tiene salud 
    private bool m_Dead;            


    private void Awake()
    {
        //Instanciamos el prefab de la explosion
        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();
        //Referencia de la fuente de audio para la explosion
        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();
        //Deshabilitamos el sistema de particulas de la explosion
        // para activarlo cuando explote
        m_ExplosionParticles.gameObject.SetActive(false);
    }


    private void OnEnable()
    {
        //Al habilitar el tanque, reseteamos la salud y el booleano
        //de si esta muerto o no.
        m_CurrentHealth = m_StartingHealth;
        m_Dead = false;
        //Actualizamos el slider de salud, valor u color
        SetHealthUI();
    }
    

    public void TakeDamage(float amount)
    {
        // Adjust the tank's current health, update the UI based on the new health and check whether or not the tank is dead.
        //Reducimos la salud segun la cantidad de damage recibido
        m_CurrentHealth-=amount;

        //Actualizamos el slider de salud con esos valores

        SetHealthUI();

        //si la salud es menor que 0 y aun no lo he explotado
        //llamo al metodo onDeath al morir 

        if (m_CurrentHealth <=0f && !m_Dead)
        {
            OnDeath();
        }
    }


    private void SetHealthUI()
    {
        // Adjust the value and colour of the slider.
        //Ajusto el valor del slider
        m_Slider.value=m_CurrentHealth;

        //Creo un color para el slider verde y rojo en funcion
        //del porcentaje de salud

        m_FillImage.color=Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth/m_StartingHealth);

    }


    private void OnDeath()
    {
        // Play the effects for the death of the tank and deactivate it.
        //Configuro el bool a true para asegurarme de que explota 
        //una sola vez

        m_Dead=true;

        //coloco el prefab de explosion en la posicion actual del tanque 
        // y lo activo.

        m_ExplosionParticles.transform.position=transform.position;

        m_ExplosionParticles.gameObject.SetActive(true);

        //Reproduzco el sistemas de particulas del tanque explotando 
        m_ExplosionParticles.Play();

        //Reproduzco el audio del tanque explotando
        m_ExplosionAudio.Play();

        //Desactivo el tanque 

        gameObject.SetActive(false);
    }
}