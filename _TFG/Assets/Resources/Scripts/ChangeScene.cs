using UnityEngine;
using UnityEngine.SceneManagement;
public class ChangeScene : MonoBehaviour
{
    //public static ChangeScene Instance;
 /*   private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }*/
    public void Cambiar_A_Escena(string nombreEscena)
    {
        SceneManager.LoadScene(nombreEscena);
    }
}
