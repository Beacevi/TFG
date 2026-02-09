using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Light2DPulse : MonoBehaviour
{
    [Header("Light Reference")]
    public Light2D light2D;

    [Header("Intensity Pulse")]
    public bool pulseIntensity = true;
    public float intensityMin = 0.5f;
    public float intensityMax = 1.5f;
    public float intensitySpeed = 2f;

    [Header("Inner Radius Pulse")]
    public bool pulseInnerRadius = false;
    public float innerRadiusMin = 0.5f;
    public float innerRadiusMax = 1.5f;
    public float innerRadiusSpeed = 2f;

    [Header("Outer Radius Pulse")]
    public bool pulseOuterRadius = false;
    public float outerRadiusMin = 1f;
    public float outerRadiusMax = 3f;
    public float outerRadiusSpeed = 2f;

    [Header("Falloff Strength Pulse")]
    public bool pulseFalloff = false;
    public float falloffMin = 0.1f;
    public float falloffMax = 1f;
    public float falloffSpeed = 2f;

    private void Reset()
    {
        light2D = GetComponent<Light2D>();
    }

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
