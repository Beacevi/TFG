using UnityEngine;
using UnityEngine.Rendering.Universal;
using System;

public class RealTimeLight : MonoBehaviour
{
    public Light2D globalLight;

    [Header("Testing")]
    public bool useFakeTime = true;

    [Range(0f, 24f)]
    public float fakeHour = 12f;

    [Header("Light Settings")]
    public float minIntensity = 0.2f;
    public float maxIntensity = 1f;

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
