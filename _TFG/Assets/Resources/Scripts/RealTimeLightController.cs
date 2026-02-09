using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RealTimeLightController : MonoBehaviour
{
    [Header("References")]
    public Light2D globalLight;

    [Header("Cycle Settings")]
    [Tooltip("Multiplier for speeding up or slowing down time. 1 = real time.")]
    public float timeScale = 1f;

    [Header("Intensity Settings")]
    public float nightIntensity = 0.1f;
    public float sunriseIntensity = 0.4f;
    public float dayIntensity = 1f;
    public float sunsetIntensity = 0.4f;

    [Header("Color Settings")]
    public Color nightColor = new Color(0.1f, 0.1f, 0.3f);
    public Color sunriseColor = new Color(1f, 0.5f, 0.3f);
    public Color dayColor = Color.white;
    public Color sunsetColor = new Color(1f, 0.4f, 0.2f);

    [Header("Time Settings (24h format)")]
    public float sunriseStart = 6f;
    public float dayStart = 8f;
    public float sunsetStart = 18f;
    public float nightStart = 20f;

    private float simulatedTime;

    void Start()
    {
        simulatedTime = System.DateTime.Now.Hour +
                        (System.DateTime.Now.Minute / 60f);
    }

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
