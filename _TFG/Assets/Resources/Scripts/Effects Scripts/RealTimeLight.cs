/**
 * @file RealTimeLight.cs
 * @brief Controla una luz en tiempo real aplicando variaciones de intensidad y color según el estado del ciclo horario.
 * @author Hortensia Studio - Beatriz Ceballos Vidal
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using UnityEngine;
using UnityEngine.Rendering.Universal;
using System;

/// <summary>
/// Controla una luz en tiempo real aplicando variaciones de intensidad y color según el estado del ciclo horario.
/// </summary>
public class RealTimeLight : MonoBehaviour
{
    /// <summary>
    /// Campo de tipo Light2D utilizado para almacenar o configurar global light.
    /// </summary>
    public Light2D globalLight;

    /// <summary>
    /// Indica si use fake time está activo o habilitado.
    /// </summary>
    [Header("Testing")]
    public bool useFakeTime = true;

    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar fake hour.
    /// </summary>
    [Range(0f, 24f)]
    public float fakeHour = 12f;

    /// <summary>
    /// Valor numérico que limita o define min intensity.
    /// </summary>
    [Header("Light Settings")]
    public float minIntensity = 0.2f;
    /// <summary>
    /// Valor numérico que limita o define max intensity.
    /// </summary>
    public float maxIntensity = 1f;

    /// <summary>
    /// Actualiza la lógica del componente en cada fotograma.
    /// </summary>
    void Update()
    {
        float hour;

        if (useFakeTime)
        {
            hour = fakeHour;
        }
        else
        {
            hour = DateTime.Now.Hour + DateTime.Now.Minute / 60f;
        }

        float normalizedTime = hour / 24f;

        float intensity =
            Mathf.Lerp(minIntensity, maxIntensity,
            Mathf.Sin(normalizedTime * Mathf.PI));

        globalLight.intensity = intensity;
    }
}
