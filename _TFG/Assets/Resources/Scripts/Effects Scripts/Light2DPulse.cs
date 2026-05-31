/**
 * @file Light2DPulse.cs
 * @brief Aplica un efecto de pulsación a una luz 2D variando su intensidad a lo largo del tiempo.
 * @author Hortensia Studio
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Aplica un efecto de pulsación a una luz 2D variando su intensidad a lo largo del tiempo.
/// </summary>
public class Light2DPulse : MonoBehaviour
{
    /// <summary>
    /// Campo de tipo Light2D utilizado para almacenar o configurar light 2 d.
    /// </summary>
    [Header("Light Reference")]
    public Light2D light2D;

    /// <summary>
    /// Indica si pulse intensity está activo o habilitado.
    /// </summary>
    [Header("Intensity Pulse")]
    public bool pulseIntensity = true;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar intensity min.
    /// </summary>
    public float intensityMin = 0.5f;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar intensity max.
    /// </summary>
    public float intensityMax = 1.5f;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar intensity speed.
    /// </summary>
    public float intensitySpeed = 2f;

    /// <summary>
    /// Indica si pulse inner radius está activo o habilitado.
    /// </summary>
    [Header("Inner Radius Pulse")]
    public bool pulseInnerRadius = false;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar inner radius min.
    /// </summary>
    public float innerRadiusMin = 0.5f;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar inner radius max.
    /// </summary>
    public float innerRadiusMax = 1.5f;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar inner radius speed.
    /// </summary>
    public float innerRadiusSpeed = 2f;

    /// <summary>
    /// Indica si pulse outer radius está activo o habilitado.
    /// </summary>
    [Header("Outer Radius Pulse")]
    public bool pulseOuterRadius = false;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar outer radius min.
    /// </summary>
    public float outerRadiusMin = 1f;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar outer radius max.
    /// </summary>
    public float outerRadiusMax = 3f;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar outer radius speed.
    /// </summary>
    public float outerRadiusSpeed = 2f;

    /// <summary>
    /// Indica si pulse falloff está activo o habilitado.
    /// </summary>
    [Header("Falloff Strength Pulse")]
    public bool pulseFalloff = false;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar falloff min.
    /// </summary>
    public float falloffMin = 0.1f;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar falloff max.
    /// </summary>
    public float falloffMax = 1f;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar falloff speed.
    /// </summary>
    public float falloffSpeed = 2f;

    /// <summary>
    /// Restablece los valores del sistema a su configuración inicial o por defecto.
    /// </summary>
    private void Reset()
    {
        light2D = GetComponent<Light2D>();
    }

    /// <summary>
    /// Actualiza la lógica del componente en cada fotograma.
    /// </summary>
    void Update()
    {
        if (light2D == null) return;

        float t;

        if (pulseIntensity)
        {
            t = (Mathf.Sin(Time.time * intensitySpeed) + 1f) * 0.5f;
            light2D.intensity = Mathf.Lerp(intensityMin, intensityMax, t);
        }

        if (pulseInnerRadius)
        {
            t = (Mathf.Sin(Time.time * innerRadiusSpeed) + 1f) * 0.5f;
            light2D.pointLightInnerRadius = Mathf.Lerp(innerRadiusMin, innerRadiusMax, t);
        }

        if (pulseOuterRadius)
        {
            t = (Mathf.Sin(Time.time * outerRadiusSpeed) + 1f) * 0.5f;
            light2D.pointLightOuterRadius = Mathf.Lerp(outerRadiusMin, outerRadiusMax, t);
        }

        if (pulseFalloff)
        {
            t = (Mathf.Sin(Time.time * falloffSpeed) + 1f) * 0.5f;
            light2D.falloffIntensity = Mathf.Lerp(falloffMin, falloffMax, t);
        }
    }
}
