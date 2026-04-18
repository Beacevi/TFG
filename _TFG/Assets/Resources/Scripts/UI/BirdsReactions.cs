using System.Collections;
using UnityEngine;

public class BirdsReactions : MonoBehaviour
{
    public BirdReactionsManager manager;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnMouseDown()
    {
        manager.TriggerReaction(this);
    }

    public void PlaySound()
    {
        if (audioSource != null)
            audioSource.Play();
    }

    public void MoveRandom(float range, float duration)
    {
        StartCoroutine(MoveRoutine(range, duration));
    }

    IEnumerator MoveRoutine(float range, float duration)
    {
        Vector3 originalPos = transform.position;

        Vector2 offset = new Vector2(
            Random.Range(-range, range),
            Random.Range(-range, range)
        );

        Vector3 target = originalPos + (Vector3)offset;

        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime / duration;
            transform.position = Vector3.Lerp(originalPos, target, t);
            yield return null;
        }

        t = 0;
        Vector3 start = transform.position;

        while (t < 1)
        {
            t += Time.deltaTime / duration;
            transform.position = Vector3.Lerp(start, originalPos, t);
            yield return null;
        }
    }
}