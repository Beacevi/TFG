using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ChangeScene : MonoBehaviour
{
  
    public void Cambiar_A_Escena(string nombreEscena)
    {
        string escenaActual = SceneManager.GetActiveScene().name;

        if (nombreEscena == escenaActual)
        {
            SceneManager.LoadScene("UI");
        }
        else
        {
            SceneManager.LoadScene(nombreEscena);
        }
    }
}
