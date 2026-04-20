using System.Collections;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
public class ChangeScene : MonoBehaviour
{
    private static UnityEngine.SceneManagement.Scene previousScene;

    [SerializeField] private Transform cloudsIzquierda;
    [SerializeField] private Transform cloudsDerecha;

    [SerializeField] private float distancia = 10f;
    [SerializeField] private float duracion = 2f;

    private void Awake()
    {
        EventSystem[] systems = FindObjectsByType<EventSystem>(FindObjectsSortMode.None);
        for (int i = 0; i < systems.Length; i++)
        {
            systems[i].gameObject.SetActive(i == 0);
        }
    }

    public void Start()
    {
        if(cloudsIzquierda!= null &&  cloudsDerecha!= null)
        {
            StartCoroutine(StartAnimation());
        }
    }

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
        Debug.Log("El nombre de la escena anterior es: "+previousScene.name);
        SceneManager.SetActiveScene(previousScene);
        SceneManager.UnloadSceneAsync(nombreEscena);
    }

    public IEnumerator StartAnimation()
    {
        yield return new WaitForSeconds(1);
        yield return StartCoroutine(AnimarNubes(true));
    }

    public IEnumerator SceneTransition(string nombreEscena, bool additive)
    {
        if (cloudsIzquierda != null && cloudsDerecha != null)
        {
            yield return StartCoroutine(AnimarNubes(false));
        }

        // pequeño margen por seguridad
        yield return new WaitForSeconds(0.2f);

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

    IEnumerator AnimarNubes(bool abrir)
    {
        Vector3 inicioIzq = cloudsIzquierda.localPosition;
        Vector3 inicioDer = cloudsDerecha.localPosition;

        Vector3 destinoIzq;
        Vector3 destinoDer;

        if (abrir)
        {
            destinoIzq = inicioIzq + Vector3.left * distancia;
            destinoDer = inicioDer + Vector3.right * distancia;
        }
        else
        {
            destinoIzq = inicioIzq + Vector3.right * distancia;
            destinoDer = inicioDer + Vector3.left * distancia;
        }

        float t = 0;

        while (t < duracion)
        {
            t += Time.deltaTime;
            float progreso = Mathf.SmoothStep(0, 1, t / duracion);

            cloudsIzquierda.localPosition = Vector3.Lerp(inicioIzq, destinoIzq, progreso);
            cloudsDerecha.localPosition = Vector3.Lerp(inicioDer, destinoDer, progreso);

            yield return null;
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
