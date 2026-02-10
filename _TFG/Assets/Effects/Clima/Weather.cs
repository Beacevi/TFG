using UnityEngine;

public class Weather : MonoBehaviour
{
    [Header("Assign all your weather particle systems here")]
    public ParticleSystem[] weatherEffects;

    [Header("Chance that ANY weather happens (0 to 1)")]
    [Range(0f, 1f)]
    public float weatherChance = 0.5f;

    void Start()
    {
        ApplyRandomWeather();
    }

    void ApplyRandomWeather()
    {
        // First decide if weather should happen at all
        bool shouldHaveWeather = Random.value < weatherChance;

        // Stop all effects first
        foreach (var effect in weatherEffects)
        {
            if (effect != null)
                effect.Stop();
        }

        if (!shouldHaveWeather)
        {
            // No weather today
            return;
        }

        // Pick a random weather effect
        int index = Random.Range(0, weatherEffects.Length);
        ParticleSystem chosenEffect = weatherEffects[index];

        if (chosenEffect != null)
            chosenEffect.Play();
    }
}
