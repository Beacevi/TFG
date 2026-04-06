using System.Collections;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ChangeScene : MonoBehaviour
{
    private static UnityEngine.SceneManagement.Scene previousScene;

   [SerializeField] private Animator animator;
    public void Cambiar_A_Escena(string nombreEscena)
    {
        string escenaActual = SceneManager.GetActiveScene().name;

        if (nombreEscena == escenaActual)
        {
            nombreEscena = "UI";
            StartCoroutine(SceneTransition(nombreEscena,false));
            
        }
        else
        {
            StartCoroutine(SceneTransition(nombreEscena,false));
        }
    }
    public void LoadSceneAdditive(string nombreEscena)
    {
        //previousScene = SceneManager.GetActiveScene();
        //SceneManager.LoadScene(nombreEscena, LoadSceneMode.Additive);

        StartCoroutine(SceneTransition(nombreEscena, true));

    }

    public void UnloadScene(string nombreEscena)
    {
        SceneManager.SetActiveScene(previousScene);
        SceneManager.UnloadSceneAsync(nombreEscena);
    }

    public IEnumerator SceneTransition(string nombreEscena, bool additive)
    {
        if (animator != null)
        {
            // Reproducir hacia delante (cerrar)
            animator.speed = 1;
            animator.Play("CloudsClosing", 0, 0f);
        }

        yield return new WaitForSeconds(3);

        if (!additive)
        {
            SceneManager.LoadScene(nombreEscena);
        }
        else
        {
            previousScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(nombreEscena, LoadSceneMode.Additive);
        }

    }

    //public void CambiarEscena(string nombreEscena, int tipo)
    //{
    //    StartCoroutine(SceneTransition(nombreEscena,tipo));
    //}



    //public IEnumerator SceneTransition(string nombreEscena, int tipo)
    //{
    //    // Reproducir hacia delante (cerrar)
    //    animator.speed = 1;
    //    animator.Play("CloudsClosing", 0, 0f);

    //    GameManager.Instance.cloudsClosing = true;

    //    yield return new WaitForSeconds(3);

    //    switch (tipo)
    //    {
    //        case 0://Cambiar_A_Escena
    //            string escenaActual = SceneManager.GetActiveScene().name;

    //            if (nombreEscena == escenaActual)
    //            {
    //                SceneManager.LoadScene("UI");
    //            }
    //            else
    //            {
    //                SceneManager.LoadScene(nombreEscena);
    //            }
    //            break;

    //        case 1://LoadSceneAdditive
    //            previousScene = SceneManager.GetActiveScene();
    //            SceneManager.LoadScene(nombreEscena, LoadSceneMode.Additive);
    //            break;

    //        case 2://UnloadScene
    //            SceneManager.SetActiveScene(previousScene);
    //            SceneManager.UnloadSceneAsync(nombreEscena);
    //            break;

    //        default:
    //            break;
    //    }

    //    // Reproducir hacia atr�s (abrir)
    //    animator.speed = -1;
    //    animator.Play("CloudsClosing", 0, 1f);

    //    GameManager.Instance.cloudsClosing = false;

    //}

}
