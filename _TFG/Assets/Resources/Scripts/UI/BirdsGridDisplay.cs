/**
 * @file BirdsGridDisplay.cs
 * @brief Construye y actualiza la cuadrícula visual de aves desbloqueadas o disponibles.
 * @author Hortensia Studio - Beatriz Ceballos Vidal
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Construye y actualiza la cuadrícula visual de aves desbloqueadas o disponibles.
/// </summary>
public class BirdsGridDisplay : MonoBehaviour
{
    /// <summary>
    /// Campo de tipo RectTransform utilizado para almacenar o configurar grid container.
    /// </summary>
    [Header("Referencias")]
    [SerializeField] private RectTransform gridContainer; // Panel con GridLayoutGroup

    /// <summary>
    /// Tiempo o duración asociado a display duration.
    /// </summary>
    [Header("Configuración")]
    [SerializeField] private float displayDuration = 3f;
    /// <summary>
    /// Tiempo o duración asociado a slide down duration.
    /// </summary>
    [SerializeField] private float slideDownDuration = 0.4f;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar delay between birds.
    /// </summary>
    [SerializeField] private float delayBetweenBirds = 0.15f;

    /// <summary>
    /// Colección de spawned birds utilizada por este componente.
    /// </summary>
    private List<RectTransform> spawnedBirds = new List<RectTransform>();

    /// <summary>
    /// Inicializa el componente cuando la escena ya está cargada y lista para comenzar.
    /// </summary>
    private void Start()
    {
        //Centrar
        //gridContainer.anchorMin = new Vector2(0.5f, 0.5f);
        //gridContainer.anchorMax = new Vector2(0.5f, 0.5f);
        //gridContainer.pivot = new Vector2(0.5f, 0.5f);
        //gridContainer.anchoredPosition = Vector2.zero;

        if (ScenePersistentManager.instance == null)
        {
            return;
        } 

        List<Bird> birds = ScenePersistentManager.instance.collectedBirdsList;
        if (birds == null || birds.Count == 0)
        {
            return;
        } 

        foreach (Bird bird in birds)
        {
            // Buscar el hijo "Normal" en el prefab del pájaro
            Transform normal = bird.birdPrefab.transform.Find("Normal");
            if (normal == null)
            {
                continue;
            } 

            SpriteRenderer sr = normal.GetComponent<SpriteRenderer>();
            if (sr == null)
            {
                continue;
            } 

            // Crear un GameObject de UI con Image
            GameObject item = new GameObject(bird.birdName, typeof(RectTransform), typeof(Image));
            item.transform.SetParent(gridContainer, false);
            item.GetComponent<Image>().sprite = sr.sprite;

            spawnedBirds.Add(item.GetComponent<RectTransform>());
        }

        StartCoroutine(DisplayAndExit());
    }

    /// <summary>
    /// Ejecuta la lógica asociada a display and exit dentro de BirdsGridDisplay.
    /// </summary>
    /// <returns>Corrutina que permite ejecutar la operación de forma diferida en Unity.</returns>
    private IEnumerator DisplayAndExit()
    {
        yield return new WaitForSeconds(displayDuration);

        foreach (RectTransform bird in spawnedBirds)
        {
            StartCoroutine(SlideDown(bird));
            yield return new WaitForSeconds(delayBetweenBirds);
        }

        // Esperar a que termine la última animación antes de limpiar
        yield return new WaitForSeconds(slideDownDuration);
        ScenePersistentManager.instance.collectedBirdsList = new List<Bird>();
    }

    /// <summary>
    /// Ejecuta la lógica asociada a slide down dentro de BirdsGridDisplay.
    /// </summary>
    /// <param name="rt">Parámetro rt empleado por el método.</param>
    /// <returns>Corrutina que permite ejecutar la operación de forma diferida en Unity.</returns>
    private IEnumerator SlideDown(RectTransform rt)
    {
        Vector2 startPos = rt.anchoredPosition;
        Vector2 endPos = startPos + Vector2.down * Screen.height;

        float elapsed = 0f;
        while (elapsed < slideDownDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / slideDownDuration);
            rt.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }

        Destroy(rt.gameObject);
    }
}
