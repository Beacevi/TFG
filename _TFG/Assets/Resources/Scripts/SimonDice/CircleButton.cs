using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CircleButton : MonoBehaviour, IPointerClickHandler
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
        if (SimonGameManager.Instance == null) return;
        if (!SimonGameManager.Instance.CanPlayerPress()) return;

        SimonGameManager.Instance.OnCirclePressed(index);

        StartCoroutine(Flash(0.2f));
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

