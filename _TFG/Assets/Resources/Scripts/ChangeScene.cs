using System.Collections;
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

    // 🔹 NUEVO: posiciones base para evitar offsets acumulados
    private Vector3 baseIzq;
    private Vector3 baseDer;

    // 🔹 NUEVO: desplazamiento dinámico según pantalla
    private float desplazamiento;
    private Camera cam;

    private void Awake()
    {
        //EventSystem[] systems = FindObjectsByType<EventSystem>(FindObjectsSortMode.None);
        //for (int i = 0; i < systems.Length; i++)
        //{
        //    systems[i].gameObject.SetActive(i == 0);
        //}
    }

    public void Start()
    {
        if(cloudsIzquierda!= null &&  cloudsDerecha!= null)
        {
            // 🔹 NUEVO
            cam = Camera.main;

            baseIzq = cloudsIzquierda.localPosition;
            baseDer = cloudsDerecha.localPosition;

            CalcularDesplazamiento();

            StartCoroutine(StartAnimation());
        }
    }

    // 🔹 NUEVO: calcula cuánto deben moverse según la cámara
    void CalcularDesplazamiento()
    {
        if (cam == null)
        {
            desplazamiento = distancia; // fallback
            return;
        }

        float altura = cam.orthographicSize * 2f;
        float anchura = altura * cam.aspect;

        desplazamiento = anchura * 0.6f;
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
        // 🔹 CAMBIADO: ya no usamos posiciones actuales ni distancia fija
        Vector3 inicioIzq;
        Vector3 destinoIzq;

        Vector3 inicioDer;
        Vector3 destinoDer;

        if (abrir)
        {
            inicioIzq = baseIzq;
            destinoIzq = baseIzq + Vector3.left * desplazamiento;

            inicioDer = baseDer;
            destinoDer = baseDer + Vector3.right * desplazamiento;
        }
        else
        {
            inicioIzq = baseIzq + Vector3.left * desplazamiento;
            destinoIzq = baseIzq;

            inicioDer = baseDer + Vector3.right * desplazamiento;
            destinoDer = baseDer;
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
}