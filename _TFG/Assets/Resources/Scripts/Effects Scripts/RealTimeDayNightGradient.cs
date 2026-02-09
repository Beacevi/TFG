using UnityEngine;

public class RealTimeDayNightPresets : MonoBehaviour
{
    [Header("Assign your material here")]
    public Material material;

    [Header("Day Colors")]
    public Color dayTop = new Color(0.35f, 0.65f, 1f);   // Bright sky blue
    public Color dayMiddle = new Color(0.55f, 0.80f, 1f);   // Light blue
    public Color dayBottom = new Color(1f, 0.95f, 0.80f);   // Warm horizon

    [Header("Sunset Colors")]
    public Color sunsetTop = new Color(1f, 0.45f, 0.35f); // Pink-orange
    public Color sunsetMiddle = new Color(1f, 0.55f, 0.25f); // Warm orange
    public Color sunsetBottom = new Color(1f, 0.35f, 0.20f); // Red-orange

    [Header("Night Colors")]
    public Color nightTop = new Color(0.05f, 0.05f, 0.15f); // Deep navy
    public Color nightMiddle = new Color(0.10f, 0.10f, 0.25f); // Dark blue
    public Color nightBottom = new Color(0.15f, 0.15f, 0.30f); // Blue-gray

    [Header("Sunrise Colors")]
    public Color sunriseTop = new Color(1f, 0.55f, 0.70f); // Soft pink
    public Color sunriseMiddle = new Color(1f, 0.70f, 0.50f); // Peach
    public Color sunriseBottom = new Color(1f, 0.85f, 0.40f); // Warm yellow

    [Header("Transition Smoothness")]
    public float smoothSpeed = 2f;

    private Color currentTop;
    private Color currentMiddle;
    private Color currentBottom;

    [Header("Global Light 2D Intensities")] 
    public UnityEngine.Rendering.Universal.Light2D globalLight; 
    public float nightIntensity = 0.2f; 
    public float sunriseIntensity = 0.5f; 
    public float dayIntensity = 1.0f; 
    public float sunsetIntensity = 0.6f;

    void Start()
    {
        float t = GetDayTime01();
        GetTargetColors(t, out currentTop, out currentMiddle, out currentBottom);
        material = GetComponent<SpriteRenderer>().material;
    }

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

    float GetDayTime01()
    {
        System.DateTime now = System.DateTime.Now;
        float secondsToday = now.Hour * 3600f + now.Minute * 60f + now.Second;
        return secondsToday / 86400f;
    }

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
