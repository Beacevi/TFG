using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    private static UnityEngine.SceneManagement.Scene previousScene;

    [SerializeField] private Transform cloudsIzquierda;
    [SerializeField] private Transform cloudsDerecha;

    // 🔥 NUEVO: control de velocidad desde Inspector
    [SerializeField, Range(0.1f, 10f)]
    private float velocidadNubes = 3f;

    private Vector3 baseIzq;
    private Vector3 baseDer;

    private float desplazamiento;
    private Camera cam;

    private int lastW;
    private int lastH;

    private void Start()
    {
        if (cloudsIzquierda != null && cloudsDerecha != null)
        {
            cam = Camera.main;

            if (cam == null)
                cam = FindFirstObjectByType<Camera>();

            baseIzq = cloudsIzquierda.localPosition;
            baseDer = cloudsDerecha.localPosition;

            CalcularDesplazamiento();

            StartCoroutine(StartAnimation());
        }
    }

    private void Update()
    {
        if (Screen.width != lastW || Screen.height != lastH)
        {
            lastW = Screen.width;
            lastH = Screen.height;

            CalcularDesplazamiento();
        }
    }

    void CalcularDesplazamiento()
    {
        if (cam == null)
            return;

        Vector3 left = cam.ViewportToWorldPoint(new Vector3(0, 0.5f, cam.nearClipPlane));
        Vector3 right = cam.ViewportToWorldPoint(new Vector3(1, 0.5f, cam.nearClipPlane));

        float width = Mathf.Abs(right.x - left.x);

        desplazamiento = width * 0.5f;
    }

    public void Cambiar_A_Escena(string nombreEscena)
    {
        string actual = SceneManager.GetActiveScene().name;

        if (nombreEscena == actual)
        {
            nombreEscena = "UI";
            StartCoroutine(SceneTransition(nombreEscena, false));
        }
        else
        {
            StartCoroutine(SceneTransition(nombreEscena, false));
        }
    }

    public void LoadSceneAdditive(string nombreEscena)
    {
        StartCoroutine(SceneTransition(nombreEscena, true));
    }

    public void UnloadScene(string nombreEscena)
    {
        Debug.Log("El nombre de la escena anterior es: " + previousScene.name);
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
        Vector3 inicioIzq, destinoIzq;
        Vector3 inicioDer, destinoDer;

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

        float t = 0f;

        while (t < 1f)
        {
            // 🔥 velocidad controlable
            t += Time.deltaTime * velocidadNubes;

            float p = Mathf.SmoothStep(0, 1, t);

            cloudsIzquierda.localPosition = Vector3.Lerp(inicioIzq, destinoIzq, p);
            cloudsDerecha.localPosition = Vector3.Lerp(inicioDer, destinoDer, p);

            yield return null;
        }
    }
}