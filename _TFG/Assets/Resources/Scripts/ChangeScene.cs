using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ChangeScene : MonoBehaviour
{
    private static UnityEngine.SceneManagement.Scene previousScene;
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
    public void LoadSceneAdditive(string nombreEscena)
    {
        previousScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(nombreEscena, LoadSceneMode.Additive);
    }

    public void UnloadScene(string nombreEscena)
    {
        SceneManager.SetActiveScene(previousScene);
        SceneManager.UnloadSceneAsync(nombreEscena);
    }

}
