using UnityEngine;

public class Weather : MonoBehaviour
{
    [Header("Assign your particle system here")]
    public ParticleSystem weatherEffect;

    [Header("Chance of weather (0 to 1)")]
    [Range(0f, 1f)]
    public float weatherChance = 0.5f;

    void Start()
    {
        ApplyRandomWeather();
    }

    void ApplyRandomWeather()
    {
        bool shouldActivate = Random.value < weatherChance;

        if (shouldActivate)
        {
            weatherEffect.Play();
        }
        else
        {
            weatherEffect.Stop();
        }
    }
}
