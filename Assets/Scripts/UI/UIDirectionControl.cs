using UnityEngine;

public class UIDirectionControl : MonoBehaviour
{
    //Se usa para asegurarnos de que los elementos del espacio UI
    //Se posicionan correctamente.
    public bool m_UseRelativeRotation = true;  


    private Quaternion m_RelativeRotation;     


    private void Start()
    {
        m_RelativeRotation = transform.parent.localRotation;
    }


    private void Update()
    {
        if (m_UseRelativeRotation)
            transform.rotation = m_RelativeRotation;
    }
}
