/**
 * @file DayNightCycle.cs
 * @brief Gestiona un ciclo día-noche básico modificando parámetros temporales o visuales de la escena.
 * @author Hortensia Studio
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Gestiona un ciclo día-noche básico modificando parámetros temporales o visuales de la escena.
/// </summary>
public class DayNightCycle : MonoBehaviour
{
    /// <summary>
    /// Campo de tipo Light2D utilizado para almacenar o configurar global light.
    /// </summary>
    [Header("References")]
    public Light2D globalLight;
    /// <summary>
    /// Campo de tipo SpriteRenderer utilizado para almacenar o configurar background.
    /// </summary>
    public SpriteRenderer background;

    /// <summary>
    /// Tiempo o duración asociado a time of day.
    /// </summary>
    [Header("Cycle Settings")]
    [Range(0f, 1f)]
    public float timeOfDay; // 0 = midnight, 0.5 = noon
    /// <summary>
    /// Tiempo o duración asociado a day duration.
    /// </summary>
    public float dayDuration = 120f; // seconds for full cycle

    /// <summary>
    /// Color utilizado para representar light color.
    /// </summary>
    [Header("Light Settings")]
    public Gradient lightColor;
    /// <summary>
    /// Campo de tipo AnimationCurve utilizado para almacenar o configurar light intensity.
    /// </summary>
    public AnimationCurve lightIntensity;

    /// <summary>
    /// Color utilizado para representar background color.
    /// </summary>
    [Header("Background Settings")]
    public Gradient backgroundColor;

    /// <summary>
    /// Actualiza la lógica del componente en cada fotograma.
    /// </summary>
    void Update()
    {
        // Advance time
        timeOfDay += Time.deltaTime / dayDuration;
        if (timeOfDay > 1f)
            timeOfDay = 0f;

        // Apply lighting
        globalLight.color = lightColor.Evaluate(timeOfDay);
        globalLight.intensity = lightIntensity.Evaluate(timeOfDay);

        // Apply background color
        background.color = backgroundColor.Evaluate(timeOfDay);
    }
}
