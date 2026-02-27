using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class CircleButtonPajaro : MonoBehaviour, IPointerClickHandler
{
    public int index;
    private Image image;
    private Color originalColor;
    public Color colorSimon;

    void Awake()
    {
        image = GetComponent<Image>();
        originalColor = image.color;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (SimonGameManagerPajaro.Instance == null) return;
        if (!SimonGameManagerPajaro.Instance.CanPlayerPress()) return;

        SimonGameManagerPajaro.Instance.OnCirclePressed(index);

        StartCoroutine(Flash(0.2f));

        Debug.Log("SimonSaysPajaro está cargada");

    }

    public IEnumerator Flash(float duration)
    {
        Color flashColor = colorSimon;
        flashColor.a = 1f;

        image.color = flashColor;
        yield return new WaitForSeconds(duration);
        image.color = originalColor;
    }

    public void SetColorInstant(Color newColor)
    {
        image.color = newColor;
    }

    public void RestoreOriginalColor()
    {
        image.color = originalColor;
    }
}

