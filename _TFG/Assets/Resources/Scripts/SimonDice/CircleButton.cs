/**
 * @file CircleButton.cs
 * @brief Representa uno de los botones circulares del minijuego tipo Simón dice.
 * @author Hortensia Studio
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Representa uno de los botones circulares del minijuego tipo Simón dice.
/// </summary>
public class CircleButton : MonoBehaviour, IPointerClickHandler
{
    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar index.
    /// </summary>
    public int index;
    /// <summary>
    /// Campo de tipo Image utilizado para almacenar o configurar image.
    /// </summary>
    private Image image;
    /// <summary>
    /// Color utilizado para representar original color.
    /// </summary>
    private Color originalColor;
    /// <summary>
    /// Color utilizado para representar color simon.
    /// </summary>
    public Color colorSimon;

    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar dim factor.
    /// </summary>
    [Tooltip("Factor (0..1) aplicado al color original cuando el botón está atenuado durante la secuencia.")]
    [SerializeField, Range(0f, 1f)] private float dimFactor = 0.35f;

    // Color de reposo "actual": coincide con originalColor en modo normal y con la versión
    // atenuada durante la secuencia. Flash() lo usa para restaurar el color al terminar.
    /// <summary>
    /// Color utilizado para representar current base color.
    /// </summary>
    private Color currentBaseColor;

    /// <summary>
    /// Inicializa referencias internas antes de que comience la ejecución normal del componente.
    /// </summary>
    void Awake()
    {
        image = GetComponent<Image>();
        originalColor = image.color;
        currentBaseColor = originalColor;
    }

    /// <summary>
    /// Procesa el clic o toque sobre un elemento de interfaz.
    /// </summary>
    /// <param name="eventData">Datos de entrada que se van a procesar.</param>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (SimonGameManager.Instance == null) return;
        if (!SimonGameManager.Instance.CanPlayerPress()) return;

        SimonGameManager.Instance.OnCirclePressed(index);

        StartCoroutine(Flash(0.2f));

        Debug.Log("SimonSays está cargada");
    }

    /// <summary>
    /// Ejecuta la lógica asociada a flash dentro de CircleButton.
    /// </summary>
    /// <param name="duration">Parámetro duration empleado por el método.</param>
    /// <returns>Corrutina que permite ejecutar la operación de forma diferida en Unity.</returns>
    public IEnumerator Flash(float duration)
    {
        Color flashColor = colorSimon;
        flashColor.a = 1f;

        image.color = flashColor;
        yield return new WaitForSeconds(duration);
        image.color = currentBaseColor;
    }

    /// <summary>
    /// Establece o actualiza color instant dentro del sistema.
    /// </summary>
    /// <param name="newColor">Parámetro new color empleado por el método.</param>
    public void SetColorInstant(Color newColor)
    {
        image.color = newColor;
    }

    /// <summary>
    /// Ejecuta la lógica asociada a restore original color dentro de CircleButton.
    /// </summary>
    public void RestoreOriginalColor()
    {
        image.color = originalColor;
        currentBaseColor = originalColor;
    }

    /// <summary>Atenúa visualmente el botón para indicar que no es pulsable (durante la secuencia).</summary>
    public void SetIdleDimmed(bool dimmed)
    {
        currentBaseColor = dimmed
            ? new Color(originalColor.r * dimFactor,
                        originalColor.g * dimFactor,
                        originalColor.b * dimFactor,
                        originalColor.a)
            : originalColor;
        image.color = currentBaseColor;
    }
}
