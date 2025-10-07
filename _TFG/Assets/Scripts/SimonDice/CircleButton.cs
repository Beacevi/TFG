using System.Collections;
using UnityEngine;

public class CircleButton : MonoBehaviour
{
    public int index;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    public Color colorSimon;

    
    //public AudioClip sound;         
    //private AudioSource audioSource;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        //audioSource = GetComponent<AudioSource>();
        originalColor = spriteRenderer.color;
    }

    void OnMouseDown()
    {
        if (SimonGameManager.Instance == null) return;
        if (!SimonGameManager.Instance.CanPlayerPress()) return;

        SimonGameManager.Instance.OnCirclePressed(index);

        // Reproduce sonido y efecto al tocar
        //PlaySound();
        StartCoroutine(Flash(0.2f));
    }

    public IEnumerator Flash(float duration)
    {
        Color flashColor = colorSimon;
        flashColor.a = 1f; // Asegurar que sea visible

        spriteRenderer.color = flashColor;
        //PlaySound();
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
    /*private void PlaySound()
    {
        if (sound != null && audioSource != null)
        {
            audioSource.PlayOneShot(sound);
        }
    }*/
}