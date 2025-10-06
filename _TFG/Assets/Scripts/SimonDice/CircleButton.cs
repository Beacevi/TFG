using System.Collections;
using UnityEngine;

public class CircleButton : MonoBehaviour
{
    public int index;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    void OnMouseDown()
    {  
        if (SimonGameManager.Instance != null)
            SimonGameManager.Instance.OnCirclePressed(index);
    }

    public IEnumerator Flash(float duration)
    {
        spriteRenderer.color = Color.yellow;
        yield return new WaitForSeconds(duration);
        spriteRenderer.color = originalColor;
    }
}