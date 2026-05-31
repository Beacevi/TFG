/**
 * @file LetterDisplayUI.cs
 * @brief Actualiza la visualización de letras o símbolos durante el minijuego musical.
 * @author Hortensia Studio
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Actualiza la visualización de letras o símbolos durante el minijuego musical.
/// </summary>
public class LetterDisplayUI : MonoBehaviour
{
    /// <summary>
    /// Instancia singleton utilizada para acceder globalmente al controlador.
    /// </summary>
    public static LetterDisplayUI Instance;

    /// <summary>
    /// Campo de tipo Image utilizado para almacenar o configurar letter image.
    /// </summary>
    [Header("UI")]
    public Image letterImage;
    /// <summary>
    /// Referencia de interfaz utilizada para mostrar o actualizar energy text.
    /// </summary>
    public TextMeshProUGUI energyText;

    /// <summary>
    /// Tiempo o duración asociado a display time.
    /// </summary>
    [Header("Animation")]
    public float displayTime = 0.6f;

    /// <summary>
    /// Campo de tipo Coroutine utilizado para almacenar o configurar current routine.
    /// </summary>
    private Coroutine currentRoutine;

    /// <summary>
    /// Inicializa referencias internas antes de que comience la ejecución normal del componente.
    /// </summary>
    void Awake()
    {
        Instance = this;

        letterImage.enabled = false;
        energyText.enabled = false;
    }

    /// <summary>
    /// Muestra letter en la interfaz o en la escena.
    /// </summary>
    /// <param name="sprite">Parámetro sprite empleado por el método.</param>
    public void ShowLetter(Sprite sprite)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowLetterRoutine(sprite));
    }

    /// <summary>
    /// Muestra letter routine en la interfaz o en la escena.
    /// </summary>
    /// <param name="sprite">Parámetro sprite empleado por el método.</param>
    /// <returns>Corrutina que permite ejecutar la operación de forma diferida en Unity.</returns>
    IEnumerator ShowLetterRoutine(Sprite sprite)
    {
        energyText.enabled = false;

        letterImage.sprite = sprite;

        letterImage.SetNativeSize();
        letterImage.rectTransform.sizeDelta = new Vector2(100, 100);

        letterImage.enabled = true;

        // Reset visual
        letterImage.color = new Color(1, 1, 1, 1);
        letterImage.transform.localScale = Vector3.zero;

        // Animaci�n pop
        float scaleTime = 0.2f;
        float t = 0;

        while (t < scaleTime)
        {
            t += Time.deltaTime;

            float scale = Mathf.Lerp(0, 1, t / scaleTime);

            letterImage.transform.localScale = Vector3.one * scale;

            yield return null;
        }

        letterImage.transform.localScale = Vector3.one;

        yield return new WaitForSeconds(displayTime);

        // Fade out
        float fadeTime = 0.3f;

        t = 0;

        Color c = letterImage.color;

        while (t < fadeTime)
        {
            t += Time.deltaTime;

            c.a = Mathf.Lerp(1, 0, t / fadeTime);

            letterImage.color = c;

            yield return null;
        }

        letterImage.enabled = false;
    }

    /// <summary>
    /// Muestra energy en la interfaz o en la escena.
    /// </summary>
    /// <param name="energy">Parámetro energy empleado por el método.</param>
    public void ShowEnergy(int energy)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        StartCoroutine(ShowEnergyRoutine(energy));
    }

    /// <summary>
    /// Muestra energy routine en la interfaz o en la escena.
    /// </summary>
    /// <param name="energy">Parámetro energy empleado por el método.</param>
    /// <returns>Corrutina que permite ejecutar la operación de forma diferida en Unity.</returns>
    IEnumerator ShowEnergyRoutine(int energy)
    {
        letterImage.enabled = false;

        energyText.enabled = true;

        energyText.text = $"+{energy} Energy";

        energyText.color = new Color(1, 1, 1, 1);

        energyText.transform.localScale = Vector3.zero;

        // Animaci�n
        float scaleTime = 0.25f;
        float t = 0;

        while (t < scaleTime)
        {
            t += Time.deltaTime;

            float scale = Mathf.Lerp(0, 1, t / scaleTime);

            energyText.transform.localScale = Vector3.one * scale;

            yield return null;
        }

        energyText.transform.localScale = Vector3.one;
    }
}
