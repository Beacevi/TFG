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

    [Tooltip("Factor (0..1) aplicado al color original cuando el botón está atenuado durante la secuencia.")]
    [SerializeField, Range(0f, 1f)] private float dimFactor = 0.35f;

    // Color de reposo "actual": coincide con originalColor en modo normal y con la versión
    // atenuada durante la secuencia. Flash() lo usa para restaurar el color al terminar.
    private Color currentBaseColor;

    void Awake()
    {
        image = GetComponent<Image>();
        originalColor = image.color;
        currentBaseColor = originalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (SimonGameManager.Instance == null) return;
        if (!SimonGameManager.Instance.CanPlayerPress()) return;

        SimonGameManager.Instance.OnCirclePressed(index);

        StartCoroutine(Flash(0.2f));

        Debug.Log("SimonSays está cargada");
    }

    public IEnumerator Flash(float duration)
    {
        Color flashColor = colorSimon;
        flashColor.a = 1f;

        image.color = flashColor;
        yield return new WaitForSeconds(duration);
        image.color = currentBaseColor;
    }

    public void SetColorInstant(Color newColor)
    {
        image.color = newColor;
    }

    public void RestoreOriginalColor()
    {
        image.color = originalColor;
        currentBaseColor = originalColor;
    }

    /// <summary>Atenúa visualmente el botón para indicar que no es pulsable (durante la secuencia).</summary>
    public void SetIdleDimmed(bool dimmed)
    {
        currentBaseColor = dimmed
            ? new Color(originalColor.r * dimFactor,
                        originalColor.g * dimFactor,
                        originalColor.b * dimFactor,
                        originalColor.a)
            : originalColor;
        image.color = currentBaseColor;
    }
}
