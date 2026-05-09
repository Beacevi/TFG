using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LetterDisplayUI : MonoBehaviour
{
    public static LetterDisplayUI Instance;

    [Header("UI")]
    public Image letterImage;
    public TextMeshProUGUI energyText;

    [Header("Animation")]
    public float displayTime = 0.6f;

    private Coroutine currentRoutine;

    void Awake()
    {
        Instance = this;

        letterImage.enabled = false;
        energyText.enabled = false;
    }

    public void ShowLetter(Sprite sprite)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowLetterRoutine(sprite));
    }

    IEnumerator ShowLetterRoutine(Sprite sprite)
    {
        energyText.enabled = false;

        letterImage.sprite = sprite;

        letterImage.SetNativeSize();
        letterImage.rectTransform.sizeDelta = new Vector2(100, 100);

        letterImage.enabled = true;

        // Reset visual
        letterImage.color = new Color(1, 1, 1, 1);
        letterImage.transform.localScale = Vector3.zero;

        // Animación pop
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

    public void ShowEnergy(int energy)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        StartCoroutine(ShowEnergyRoutine(energy));
    }

    IEnumerator ShowEnergyRoutine(int energy)
    {
        letterImage.enabled = false;

        energyText.enabled = true;

        energyText.text = $"+{energy} Energy";

        energyText.color = new Color(1, 1, 1, 1);

        energyText.transform.localScale = Vector3.zero;

        // Animación
        float scaleTime = 0.25f;
        float t = 0;

        while (t < scaleTime)
        {
            t += Time.deltaTime;

            float scale = Mathf.Lerp(0, 1, t / scaleTime);

            energyText.transform.localScale = Vector3.one * scale;

            yield return null;
        }

        energyText.transform.localScale = Vector3.one;
    }
}