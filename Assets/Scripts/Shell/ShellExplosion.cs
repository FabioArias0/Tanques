using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    //Usado para filtrar a que afecta la explosion de la bomba
    // Deberia ajustarse a player
    public LayerMask m_TankMask;

    //Referencia a las particulas que se reproduciran en la explosion
    public ParticleSystem m_ExplosionParticles;   
    //Referencia al audio que se reproducira en la explosion    
    public AudioSource m_ExplosionAudio;    
    //Cantidad de damage si la explosion esta cerrada en el tanque          
    public float m_MaxDamage = 100f;
    //Cantidad de fuerza añadida al tanque en el centro de la explosion                  
    public float m_ExplosionForce = 1000f;  
    //Tiempo de vida en segundos de la bomba           
    public float m_MaxLifeTime = 2f;      
    //Radio max desde la explosion para calcular los tanques
    //se veran afectados            
    public float m_ExplosionRadius = 5f;              


    private void Start()
    {
        //Si no se ha destruido aun, destruir la bomba despues
        //de su tiempo de vida
        Destroy(gameObject, m_MaxLifeTime);
    }


    private void OnTriggerEnter(Collider other)
    {
        // Find all the tanks in an area around the shell and damage them.
        //Recoge los colliders en una esfera desde la posicion de la 
        //Bomba con el radio max

        Collider[] colliders = Physics.OverlapSphere(transform.position,m_ExplosionRadius,m_TankMask);

        //Recorro los colliders

        for (int i = 0; i < colliders.Length; i++)
        {
            //Selecciono su RigidBody
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();


            //Si no tienen, paso al siguiente.

            if (!targetRigidbody)
            continue;


            //Add la fuerza de la explosion

            targetRigidbody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);

            //Busco el script TankHealth asociado con el rigidbody
            TankHealth targetHealth=targetRigidbody.GetComponent<TankHealth>();


            //Si no hay script Tankhealth, paso al siguiente
            if(!targetHealth)
            continue;

            //Calculo el damage a aplicar en funcion de la distancia a la bomba
            float damage = CalculateDamage(targetRigidbody.position);


            //Aplico el damge al tanque
            targetHealth.TakeDamage(damage);
                           
            
        }

        //Desanclo el sistema de particulas de la bomba
        m_ExplosionParticles.transform.parent=null;

        //Reproduzco el sistema de particulas
        m_ExplosionParticles.Play();

        //Reproduzco el audio
        m_ExplosionAudio.Play();

        //Cuando las particulas han terminado, destryo su objeto asociado


        Destroy(m_ExplosionParticles.gameObject, m_ExplosionParticles.main.duration);

        //Destruyo la bomba
        Destroy(gameObject);
    }


    private float CalculateDamage(Vector3 targetPosition){

        //Creo un vector desde la bomba al objetivo
        Vector3 explosionToTarget = targetPosition-transform.position;


        //Calculo la distancia desde la bomba al objetivo
        float explosionDistance = explosionToTarget.magnitude;

        //Calculo la proporcion de max distancia, radio maximo, desde
        //la explosion al tanque

        float relativeDistance = (m_ExplosionRadius-explosionDistance)/m_ExplosionRadius;

        //Calculo el damage a ese proporcion 
        float damage = relativeDistance*m_MaxDamage;

        //Me aseguro de que le min damage siempre es 0
        damage = Mathf.Max(0f,damage);

        //Devuelvo el damage
        return damage;
    }


    
}