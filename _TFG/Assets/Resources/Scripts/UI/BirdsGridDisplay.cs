using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BirdsGridDisplay : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private RectTransform gridContainer; // Panel con GridLayoutGroup

    [Header("Configuración")]
    [SerializeField] private float displayDuration = 3f;
    [SerializeField] private float slideDownDuration = 0.4f;
    [SerializeField] private float delayBetweenBirds = 0.15f;

    private List<RectTransform> spawnedBirds = new List<RectTransform>();

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