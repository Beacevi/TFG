using UnityEngine;
using UnityEngine.SceneManagement;
public class CambiarEscena : MonoBehaviour
{
    public void Cambiar_A_Escena(string nombreEscena)
    {
        SceneManager.LoadScene(nombreEscena);
    }
}
