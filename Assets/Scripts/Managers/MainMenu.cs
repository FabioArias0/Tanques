using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Jugar() {
        SceneManager.LoadScene(1);
        Debug.Log("Estas haciendo clic en el boton jugar");
    }
}
