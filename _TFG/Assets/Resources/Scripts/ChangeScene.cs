/**
 * @file ChangeScene.cs
 * @brief Centraliza los cambios de escena del proyecto, incluyendo transiciones visuales y llamadas desde botones de interfaz.
 * @author Hortensia Studio - David Díaz Espinosa de los Monteros
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/// <summary>
/// Centraliza los cambios de escena del proyecto, incluyendo transiciones visuales y llamadas desde botones de interfaz.
/// </summary>
public class ChangeScene : MonoBehaviour
{
    /// <summary>
    /// Campo de tipo UnityEngine.SceneManagement.Scene utilizado para almacenar o configurar previous scene.
    /// </summary>
    private static UnityEngine.SceneManagement.Scene previousScene;

    /// <summary>
    /// Campo de tipo Transform utilizado para almacenar o configurar clouds izquierda.
    /// </summary>
    [SerializeField] private Transform cloudsIzquierda;
    /// <summary>
    /// Campo de tipo Transform utilizado para almacenar o configurar clouds derecha.
    /// </summary>
    [SerializeField] private Transform cloudsDerecha;

    //NUEVO: control de velocidad desde Inspector
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar velocidad nubes.
    /// </summary>
    [SerializeField, Range(0.1f, 10f)]
    private float velocidadNubes = 3f;

    /// <summary>
    /// Campo de tipo Vector3 utilizado para almacenar o configurar base izq.
    /// </summary>
    private Vector3 baseIzq;
    /// <summary>
    /// Campo de tipo Vector3 utilizado para almacenar o configurar base der.
    /// </summary>
    private Vector3 baseDer;

    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar desplazamiento.
    /// </summary>
    private float desplazamiento;
    /// <summary>
    /// Campo de tipo Camera utilizado para almacenar o configurar cam.
    /// </summary>
    private Camera cam;

    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar last w.
    /// </summary>
    private int lastW;
    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar last h.
    /// </summary>
    private int lastH;

    /// <summary>
    /// Inicializa el componente cuando la escena ya está cargada y lista para comenzar.
    /// </summary>
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

    /// <summary>
    /// Actualiza la lógica del componente en cada fotograma.
    /// </summary>
    private void Update()
    {
        if (Screen.width != lastW || Screen.height != lastH)
        {
            lastW = Screen.width;
            lastH = Screen.height;

            CalcularDesplazamiento();
        }
    }

    /// <summary>
    /// Ejecuta la lógica asociada a calcular desplazamiento dentro de ChangeScene.
    /// </summary>
    void CalcularDesplazamiento()
    {
        if (cam == null)
            return;

        Vector3 left = cam.ViewportToWorldPoint(new Vector3(0, 0.5f, cam.nearClipPlane));
        Vector3 right = cam.ViewportToWorldPoint(new Vector3(1, 0.5f, cam.nearClipPlane));

        float width = Mathf.Abs(right.x - left.x);

        desplazamiento = width * 0.5f;
    }

    /// <summary>
    /// Cambia cambiar a escena según la interacción o parámetro recibido.
    /// </summary>
    /// <param name="nombreEscena">Parámetro nombre escena empleado por el método.</param>
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

    /// <summary>
    /// Carga el estado previamente guardado y restaura los datos del sistema.
    /// </summary>
    /// <param name="nombreEscena">Parámetro nombre escena empleado por el método.</param>
    public void LoadSceneAdditive(string nombreEscena)
    {
        StartCoroutine(SceneTransition(nombreEscena, true));
    }

    /// <summary>
    /// Ejecuta la lógica asociada a unload scene dentro de ChangeScene.
    /// </summary>
    /// <param name="nombreEscena">Parámetro nombre escena empleado por el método.</param>
    public void UnloadScene(string nombreEscena)
    {
        Debug.Log("El nombre de la escena anterior es: " + previousScene.name);
        SceneManager.SetActiveScene(previousScene);
        SceneManager.UnloadSceneAsync(nombreEscena);
    }

    /// <summary>
    /// Ejecuta la lógica asociada a start animation dentro de ChangeScene.
    /// </summary>
    /// <returns>Corrutina que permite ejecutar la operación de forma diferida en Unity.</returns>
    public IEnumerator StartAnimation()
    {
        yield return new WaitForSeconds(1);
        yield return StartCoroutine(AnimarNubes(true));
    }

    /// <summary>
    /// Ejecuta la lógica asociada a scene transition dentro de ChangeScene.
    /// </summary>
    /// <param name="nombreEscena">Parámetro nombre escena empleado por el método.</param>
    /// <param name="additive">Parámetro additive empleado por el método.</param>
    /// <returns>Corrutina que permite ejecutar la operación de forma diferida en Unity.</returns>
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

    /// <summary>
    /// Ejecuta la lógica asociada a animar nubes dentro de ChangeScene.
    /// </summary>
    /// <param name="abrir">Parámetro abrir empleado por el método.</param>
    /// <returns>Corrutina que permite ejecutar la operación de forma diferida en Unity.</returns>
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
            // velocidad controlable
            t += Time.deltaTime * velocidadNubes;

            float p = Mathf.SmoothStep(0, 1, t);

            cloudsIzquierda.localPosition = Vector3.Lerp(inicioIzq, destinoIzq, p);
            cloudsDerecha.localPosition = Vector3.Lerp(inicioDer, destinoDer, p);

            yield return null;
        }
    }
}
