/**
 * @file RealTimeDayNightGradient.cs
 * @brief Almacena una colección de presets visuales para representar distintas franjas del ciclo día-noche.
 * @author Hortensia Studio - Beatriz Ceballos Vidal
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using UnityEngine;

/// <summary>
/// Almacena una colección de presets visuales para representar distintas franjas del ciclo día-noche.
/// </summary>
public class RealTimeDayNightPresets : MonoBehaviour
{
    /// <summary>
    /// Campo de tipo Material utilizado para almacenar o configurar material.
    /// </summary>
    [Header("Assign your material here")]
    public Material material;

    /// <summary>
    /// Campo de tipo Color utilizado para almacenar o configurar day top.
    /// </summary>
    [Header("Day Colors")]
    public Color dayTop = new Color(0.35f, 0.65f, 1f);   // Bright sky blue
    /// <summary>
    /// Campo de tipo Color utilizado para almacenar o configurar day middle.
    /// </summary>
    public Color dayMiddle = new Color(0.55f, 0.80f, 1f);   // Light blue
    /// <summary>
    /// Campo de tipo Color utilizado para almacenar o configurar day bottom.
    /// </summary>
    public Color dayBottom = new Color(1f, 0.95f, 0.80f);   // Warm horizon

    /// <summary>
    /// Campo de tipo Color utilizado para almacenar o configurar sunset top.
    /// </summary>
    [Header("Sunset Colors")]
    public Color sunsetTop = new Color(1f, 0.45f, 0.35f); // Pink-orange
    /// <summary>
    /// Campo de tipo Color utilizado para almacenar o configurar sunset middle.
    /// </summary>
    public Color sunsetMiddle = new Color(1f, 0.55f, 0.25f); // Warm orange
    /// <summary>
    /// Campo de tipo Color utilizado para almacenar o configurar sunset bottom.
    /// </summary>
    public Color sunsetBottom = new Color(1f, 0.35f, 0.20f); // Red-orange

    /// <summary>
    /// Campo de tipo Color utilizado para almacenar o configurar night top.
    /// </summary>
    [Header("Night Colors")]
    public Color nightTop = new Color(0.05f, 0.05f, 0.15f); // Deep navy
    /// <summary>
    /// Campo de tipo Color utilizado para almacenar o configurar night middle.
    /// </summary>
    public Color nightMiddle = new Color(0.10f, 0.10f, 0.25f); // Dark blue
    /// <summary>
    /// Campo de tipo Color utilizado para almacenar o configurar night bottom.
    /// </summary>
    public Color nightBottom = new Color(0.15f, 0.15f, 0.30f); // Blue-gray

    /// <summary>
    /// Campo de tipo Color utilizado para almacenar o configurar sunrise top.
    /// </summary>
    [Header("Sunrise Colors")]
    public Color sunriseTop = new Color(1f, 0.55f, 0.70f); // Soft pink
    /// <summary>
    /// Campo de tipo Color utilizado para almacenar o configurar sunrise middle.
    /// </summary>
    public Color sunriseMiddle = new Color(1f, 0.70f, 0.50f); // Peach
    /// <summary>
    /// Campo de tipo Color utilizado para almacenar o configurar sunrise bottom.
    /// </summary>
    public Color sunriseBottom = new Color(1f, 0.85f, 0.40f); // Warm yellow

    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar smooth speed.
    /// </summary>
    [Header("Transition Smoothness")]
    public float smoothSpeed = 2f;

    /// <summary>
    /// Campo de tipo Color utilizado para almacenar o configurar current top.
    /// </summary>
    private Color currentTop;
    /// <summary>
    /// Campo de tipo Color utilizado para almacenar o configurar current middle.
    /// </summary>
    private Color currentMiddle;
    /// <summary>
    /// Campo de tipo Color utilizado para almacenar o configurar current bottom.
    /// </summary>
    private Color currentBottom;

    /// <summary>
    /// Campo de tipo UnityEngine.Rendering.Universal.Light2D utilizado para almacenar o configurar global light.
    /// </summary>
    [Header("Global Light 2D Intensities")] 
    public UnityEngine.Rendering.Universal.Light2D globalLight; 
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar night intensity.
    /// </summary>
    public float nightIntensity = 0.2f; 
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar sunrise intensity.
    /// </summary>
    public float sunriseIntensity = 0.5f; 
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar day intensity.
    /// </summary>
    public float dayIntensity = 1.0f; 
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar sunset intensity.
    /// </summary>
    public float sunsetIntensity = 0.6f;

    /// <summary>
    /// Inicializa el componente cuando la escena ya está cargada y lista para comenzar.
    /// </summary>
    void Start()
    {
        float t = GetDayTime01();
        GetTargetColors(t, out currentTop, out currentMiddle, out currentBottom);
        material = GetComponent<SpriteRenderer>().material;
    }

    /// <summary>
    /// Actualiza la lógica del componente en cada fotograma.
    /// </summary>
    void Update()
    {
        float t = GetDayTime01();

        Color targetTop, targetMiddle, targetBottom;
        GetTargetColors(t, out targetTop, out targetMiddle, out targetBottom);

        currentTop = Color.Lerp(currentTop, targetTop, Time.deltaTime * smoothSpeed);
        currentMiddle = Color.Lerp(currentMiddle, targetMiddle, Time.deltaTime * smoothSpeed);
        currentBottom = Color.Lerp(currentBottom, targetBottom, Time.deltaTime * smoothSpeed);

        material.SetColor("_ColorTop", currentTop);
        material.SetColor("_ColorMiddle", currentMiddle);
        material.SetColor("_ColorBottom", currentBottom);

        // Smoothly transition global light intensity
        globalLight.intensity = Mathf.Lerp(globalLight.intensity, GetTargetIntensity(t), Time.deltaTime * smoothSpeed);

    }

    /// <summary>
    /// Obtiene day time 01 a partir del estado actual del sistema.
    /// </summary>
    /// <returns>Valor numérico calculado o consultado por el método.</returns>
    float GetDayTime01()
    {
        System.DateTime now = System.DateTime.Now;
        float secondsToday = now.Hour * 3600f + now.Minute * 60f + now.Second;
        return secondsToday / 86400f;
    }

    /// <summary>
    /// Obtiene target colors a partir del estado actual del sistema.
    /// </summary>
    /// <param name="t">Parámetro t empleado por el método.</param>
    /// <param name="top">Parámetro top empleado por el método.</param>
    /// <param name="middle">Parámetro middle empleado por el método.</param>
    /// <param name="bottom">Parámetro bottom empleado por el método.</param>
    void GetTargetColors(float t, out Color top, out Color middle, out Color bottom)
    {
        float sunriseStart = 0.20f; // ~05:00
        float dayStart = 0.30f; // ~07:00
        float sunsetStart = 0.70f; // ~17:00
        float nightStart = 0.85f; // ~20:00

        if (t < sunriseStart) // Night ? Sunrise
        {
            float f = Mathf.InverseLerp(0f, sunriseStart, t);
            top = Color.Lerp(nightTop, sunriseTop, f);
            middle = Color.Lerp(nightMiddle, sunriseMiddle, f);
            bottom = Color.Lerp(nightBottom, sunriseBottom, f);
        }
        else if (t < dayStart) // Sunrise ? Day
        {
            float f = Mathf.InverseLerp(sunriseStart, dayStart, t);
            top = Color.Lerp(sunriseTop, dayTop, f);
            middle = Color.Lerp(sunriseMiddle, dayMiddle, f);
            bottom = Color.Lerp(sunriseBottom, dayBottom, f);
        }
        else if (t < sunsetStart) // Day ? Sunset
        {
            float f = Mathf.InverseLerp(dayStart, sunsetStart, t);
            top = Color.Lerp(dayTop, sunsetTop, f);
            middle = Color.Lerp(dayMiddle, sunsetMiddle, f);
            bottom = Color.Lerp(dayBottom, sunsetBottom, f);
        }
        else if (t < nightStart) // Sunset ? Night
        {
            float f = Mathf.InverseLerp(sunsetStart, nightStart, t);
            top = Color.Lerp(sunsetTop, nightTop, f);
            middle = Color.Lerp(sunsetMiddle, nightMiddle, f);
            bottom = Color.Lerp(sunsetBottom, nightBottom, f);
        }
        else // Night
        {
            top = nightTop;
            middle = nightMiddle;
            bottom = nightBottom;
        }
    }
    /// <summary>
    /// Obtiene target intensity a partir del estado actual del sistema.
    /// </summary>
    /// <param name="t">Parámetro t empleado por el método.</param>
    /// <returns>Valor numérico calculado o consultado por el método.</returns>
    float GetTargetIntensity(float t)
    {
        float sunriseStart = 0.20f; float dayStart = 0.30f; float sunsetStart = 0.70f; float nightStart = 0.85f; if (t < sunriseStart) // Night ? Sunrise
        { float f = Mathf.InverseLerp(0f, sunriseStart, t); return Mathf.Lerp(nightIntensity, sunriseIntensity, f); } else if (t < dayStart) // Sunrise ? Day
        { float f = Mathf.InverseLerp(sunriseStart, dayStart, t); return Mathf.Lerp(sunriseIntensity, dayIntensity, f); } else if (t < sunsetStart) // Day ? Sunset
        { float f = Mathf.InverseLerp(dayStart, sunsetStart, t); return Mathf.Lerp(dayIntensity, sunsetIntensity, f); } else if (t < nightStart) // Sunset ? Night
        { float f = Mathf.InverseLerp(sunsetStart, nightStart, t); return Mathf.Lerp(sunsetIntensity, nightIntensity, f); } else // Night
        { return nightIntensity; }
                                                                                                                                 }
}
