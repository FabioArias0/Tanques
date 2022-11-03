using UnityEngine;

public class CameraControl : MonoBehaviour
{
    //tiempo de espera para mover la camara
    public float m_DampTime = 0.2f;     
    // padding de menor tamaño para que los tanques no se 
    //peguen a los bordes        
    public float m_ScreenEdgeBuffer = 4f;  
    //tamaño minimo de zoom         
    public float m_MinSize = 6.5f;          
    //array de tanques, no se mostraran en el inspector cuando
    //haya Game Manager        
    [HideInInspector] public Transform[] m_Targets; 

    //camara
    private Camera m_Camera;   
    //velocidad del zoom                     
    private float m_ZoomSpeed;    
    //velocidad del movimiento                  
    private Vector3 m_MoveVelocity;   
    //posicion a la que quiero llegar              
    private Vector3 m_DesiredPosition;              


    private void Awake()
    {
        //iniciamos la camara al arrancar
        m_Camera = GetComponentInChildren<Camera>();
    }


    private void FixedUpdate()
    {
        Move(); //mueve la camara
        Zoom();// le ajusta el zoom a la camara
    }


    private void Move()
    {
        //Busco la posicion media entre los dos tanques
        FindAveragePosition();
        //Muevo la camara de forma suave
        transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
    }


    private void FindAveragePosition()
    {
        Vector3 averagePos = new Vector3();
        int numTargets = 0;
        //Recorre la cantidad de los tanques activos, captura su
        //posicion y asigna a m_DesirePosition el punto medio de ellos
        // en el eje y

        for (int i = 0; i < m_Targets.Length; i++)
        {
            //Si no esta activo me lo salto
            if (!m_Targets[i].gameObject.activeSelf)
                continue;
            // incremento el valor de la media y el # de elementos
            averagePos += m_Targets[i].position;
            numTargets++;
        }
        //SI hay elementos hago la media
        if (numTargets > 0)
            averagePos /= numTargets;
        //Mantengo el valor de y
        averagePos.y = transform.position.y;
        //La posicion deseada de la media
        m_DesiredPosition = averagePos;
    }


    private void Zoom()
    {
        //Busca la posicion requerida de zoom¨size¨ y la asignamos a camara
        float requiredSize = FindRequiredSize();
        //Ajusto el size de la camara de forma suave
        m_Camera.orthographicSize = Mathf.SmoothDamp(m_Camera.orthographicSize, requiredSize, ref m_ZoomSpeed, m_DampTime);
    }


    private float FindRequiredSize()
    {
        //Teniendo en cuanta la posicion deseada
        Vector3 desiredLocalPos = transform.InverseTransformPoint(m_DesiredPosition);

        float size = 0f;
        //Recorremos los tanques activos y tomamos la posicion más alta,
        //el que esta mas lejos del centro
        for (int i = 0; i < m_Targets.Length; i++)
        {
            //Se no esta activo me lo salto
            if (!m_Targets[i].gameObject.activeSelf)
                continue;
            //Posicion del tanque en el espacio de la camara
            Vector3 targetLocalPos = transform.InverseTransformPoint(m_Targets[i].position);
            //Diferencia entre la deseada y la actual
            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;
            //Elijo el maximo entre el size de la camara actual y la distancia
            //del tanque , arriba o abajo
            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.y));
            //Elijo el maximo entre el size de camara actual y la distancia del tanque 
            //izquierda o derecha
            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.x) / m_Camera.aspect);
        }
        
        //Aplica padding
        size += m_ScreenEdgeBuffer;
        //Comprobamos que al menos tenemos zoom minimo
        size = Mathf.Max(size, m_MinSize);

        return size;
    }


    //La usaremos en el GameMake para resetear la posicion y el zoom en cada
    //escena
    public void SetStartPositionAndSize()
    {
        //Buscamos la posicion deseada
        FindAveragePosition();

        //Ajustamos la posicion de la camara
        //sin padding porque va ser al entrar
        transform.position = m_DesiredPosition;
        //Buscamos y ajustamos el size de la camara
        m_Camera.orthographicSize = FindRequiredSize();
    }
}