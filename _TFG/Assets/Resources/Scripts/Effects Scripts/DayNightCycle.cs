using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayNightCycle : MonoBehaviour
{
    [Header("References")]
    public Light2D globalLight;
    public SpriteRenderer background;

    [Header("Cycle Settings")]
    [Range(0f, 1f)]
    public float timeOfDay; // 0 = midnight, 0.5 = noon
    public float dayDuration = 120f; // seconds for full cycle

    [Header("Light Settings")]
    public Gradient lightColor;
    public AnimationCurve lightIntensity;

    [Header("Background Settings")]
    public Gradient backgroundColor;

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
