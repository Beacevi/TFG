using UnityEngine;
using UnityEngine.SceneManagement;
public class ChangeScene : MonoBehaviour
{
    public void Cambiar_A_Escena(string nombreEscena)
    {
        SceneManager.LoadScene(nombreEscena);
    }
}
