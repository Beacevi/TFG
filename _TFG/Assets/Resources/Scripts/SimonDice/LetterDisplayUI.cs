using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LetterDisplayUI : MonoBehaviour
{
    public static LetterDisplayUI Instance;

    public Image letterImage;
    public float displayTime = 0.6f;

    private Coroutine currentRoutine;

    void Awake()
    {
        Instance = this;
        letterImage.enabled = false;
    }

    public void ShowLetter(Sprite sprite)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowRoutine(sprite));
    }

    IEnumerator ShowRoutine(Sprite sprite)
    {
        letterImage.sprite = sprite;
        letterImage.enabled = true;

        // Reset estado
        letterImage.color = new Color(1, 1, 1, 1);
        letterImage.transform.localScale = Vector3.zero;

        // Animación de escala manual (pop)
        float scaleTime = 0.2f;
        float t = 0;

        while (t < scaleTime)
        {
            t += Time.deltaTime;
            float scale = Mathf.Lerp(0, 1, t / scaleTime);
            letterImage.transform.localScale = Vector3.one * scale;
            yield return null;
        }

        letterImage.transform.localScale = Vector3.one;

        yield return new WaitForSeconds(displayTime);

        // Fade out
        float fadeTime = 0.3f;
        t = 0;
        Color c = letterImage.color;

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(1, 0, t / fadeTime);
            letterImage.color = c;
            yield return null;
        }

        letterImage.enabled = false;
    }
}