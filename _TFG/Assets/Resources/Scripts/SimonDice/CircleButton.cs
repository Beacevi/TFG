using System.Collections;
using UnityEngine;

public class CircleButton : MonoBehaviour
{
    public int index;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    public Color colorSimon;


    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    void OnMouseDown()
    {
        if (SimonGameManager.Instance == null) return;
        if (!SimonGameManager.Instance.CanPlayerPress()) return;

        SimonGameManager.Instance.OnCirclePressed(index);

        StartCoroutine(Flash(0.2f));
    }

    public IEnumerator Flash(float duration)
    {
        Color flashColor = colorSimon;
        flashColor.a = 1f; // Asegurar que sea visible

        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(duration);
        spriteRenderer.color = originalColor;
    }

    public void SetColorInstant(Color newColor)
    {
        spriteRenderer.color = newColor;
    }

    public void RestoreOriginalColor()
    {
        spriteRenderer.color = originalColor;
    } 
}