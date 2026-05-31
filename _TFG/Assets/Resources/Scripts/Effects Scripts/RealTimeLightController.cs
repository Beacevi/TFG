/**
 * @file RealTimeLightController.cs
 * @brief Coordina las luces de la escena para adaptar la iluminación global al ciclo temporal del juego.
 * @author Hortensia Studio
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Coordina las luces de la escena para adaptar la iluminación global al ciclo temporal del juego.
/// </summary>
public class RealTimeLightController : MonoBehaviour
{
    /// <summary>
    /// Campo de tipo Light2D utilizado para almacenar o configurar global light.
    /// </summary>
    [Header("References")]
    public Light2D globalLight;

    /// <summary>
    /// Tiempo o duración asociado a time scale.
    /// </summary>
    [Header("Cycle Settings")]
    [Tooltip("Multiplier for speeding up or slowing down time. 1 = real time.")]
    public float timeScale = 1f;

    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar night intensity.
    /// </summary>
    [Header("Intensity Settings")]
    public float nightIntensity = 0.1f;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar sunrise intensity.
    /// </summary>
    public float sunriseIntensity = 0.4f;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar day intensity.
    /// </summary>
    public float dayIntensity = 1f;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar sunset intensity.
    /// </summary>
    public float sunsetIntensity = 0.4f;

    /// <summary>
    /// Color utilizado para representar night color.
    /// </summary>
    [Header("Color Settings")]
    public Color nightColor = new Color(0.1f, 0.1f, 0.3f);
    /// <summary>
    /// Color utilizado para representar sunrise color.
    /// </summary>
    public Color sunriseColor = new Color(1f, 0.5f, 0.3f);
    /// <summary>
    /// Color utilizado para representar day color.
    /// </summary>
    public Color dayColor = Color.white;
    /// <summary>
    /// Color utilizado para representar sunset color.
    /// </summary>
    public Color sunsetColor = new Color(1f, 0.4f, 0.2f);

    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar sunrise start.
    /// </summary>
    [Header("Time Settings (24h format)")]
    public float sunriseStart = 6f;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar day start.
    /// </summary>
    public float dayStart = 8f;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar sunset start.
    /// </summary>
    public float sunsetStart = 18f;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar night start.
    /// </summary>
    public float nightStart = 20f;

    /// <summary>
    /// Tiempo o duración asociado a simulated time.
    /// </summary>
    private float simulatedTime;

    /// <summary>
    /// Inicializa el componente cuando la escena ya está cargada y lista para comenzar.
    /// </summary>
    void Start()
    {
        simulatedTime = System.DateTime.Now.Hour +
                        (System.DateTime.Now.Minute / 60f);
    }

    /// <summary>
    /// Actualiza la lógica del componente en cada fotograma.
    /// </summary>
    void Update()
    {
        if (globalLight == null) return;

        // Advance simulated time
        simulatedTime += Time.deltaTime * timeScale;

        // Wrap around 24h
        if (simulatedTime >= 24f)
            simulatedTime -= 24f;

        // Apply lighting
        globalLight.intensity = CalculateIntensity(simulatedTime);
        globalLight.color = CalculateColor(simulatedTime);
    }

    /// <summary>
    /// Calcula intensity a partir de los datos disponibles.
    /// </summary>
    /// <param name="hour">Parámetro hour empleado por el método.</param>
    /// <returns>Valor numérico calculado o consultado por el método.</returns>
    float CalculateIntensity(float hour)
    {
        if (hour < sunriseStart) return nightIntensity;

        if (hour < dayStart)
            return Mathf.Lerp(nightIntensity, sunriseIntensity,
                Mathf.InverseLerp(sunriseStart, dayStart, hour));

        if (hour < sunsetStart)
            return Mathf.Lerp(sunriseIntensity, dayIntensity,
                Mathf.InverseLerp(dayStart, sunsetStart, hour));

        if (hour < nightStart)
            return Mathf.Lerp(dayIntensity, sunsetIntensity,
                Mathf.InverseLerp(sunsetStart, nightStart, hour));

        return Mathf.Lerp(sunsetIntensity, nightIntensity,
            Mathf.InverseLerp(nightStart, 24f, hour));
    }

    /// <summary>
    /// Calcula color a partir de los datos disponibles.
    /// </summary>
    /// <param name="hour">Parámetro hour empleado por el método.</param>
    /// <returns>Instancia o valor de tipo Color resultante de la operación.</returns>
    Color CalculateColor(float hour)
    {
        if (hour < sunriseStart) return nightColor;

        if (hour < dayStart)
            return Color.Lerp(nightColor, sunriseColor,
                Mathf.InverseLerp(sunriseStart, dayStart, hour));

        if (hour < sunsetStart)
            return Color.Lerp(sunriseColor, dayColor,
                Mathf.InverseLerp(dayStart, sunsetStart, hour));

        if (hour < nightStart)
            return Color.Lerp(dayColor, sunsetColor,
                Mathf.InverseLerp(sunsetStart, nightStart, hour));

        return Color.Lerp(sunsetColor, nightColor,
            Mathf.InverseLerp(nightStart, 24f, hour));
    }
}
