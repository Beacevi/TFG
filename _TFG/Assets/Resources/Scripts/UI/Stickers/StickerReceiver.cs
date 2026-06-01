/**
 * @file StickerReceiver.cs
 * @brief Zona receptora que permite colocar stickers o elementos decorativos en la interfaz.
 * @author Hortensia Studio - David Díaz Espinosa de los Monteros
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using UnityEngine;

/// <summary>
/// Zona receptora que permite colocar stickers o elementos decorativos en la interfaz.
/// </summary>
public class StickerReceiver : MonoBehaviour
{
    /// <summary>
    /// Procesa la pulsación directa sobre el objeto desde el ratón o entrada equivalente.
    /// </summary>
    private void OnMouseDown()
    {
        if (!StickerManager.Instance.modoActual)
        {
            PlaceSticker();
        }

        if (StickerManager.Instance.modoActual)
        {
            RemoveSticker();
        }

    }

    /// <summary>
    /// Elimina sticker del estado gestionado por el componente.
    /// </summary>
    void RemoveSticker()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0;

        int stickerLayer = LayerMask.GetMask("Stickers");

        Collider2D[] hits = Physics2D.OverlapPointAll(worldPos, stickerLayer);

        GameObject topSticker = null;
        int highestOrder = int.MinValue;

        foreach (var hit in hits)
        {
            SpriteRenderer sr = hit.GetComponent<SpriteRenderer>();

            if (sr != null && sr.sortingOrder > highestOrder)
            {
                highestOrder = sr.sortingOrder;
                topSticker = hit.gameObject;
            }
        }

        if (topSticker != null)
        {
            Destroy(topSticker);
        }
    }

    /// <summary>
    /// Ejecuta la lógica asociada a place sticker dentro de StickerReceiver.
    /// </summary>
    void PlaceSticker()
{
    if (StickerManager.Instance != null)
    {
        var sticker = StickerManager.Instance.selectedSticker;

        if (sticker == null || sticker.amount <= 0)
        {
            return;
        }

        // Crear objeto sticker
        GameObject newSticker = new GameObject("Sticker");
        newSticker.transform.SetParent(transform);
        newSticker.tag = "Sticker";

        //Layer aquí
        newSticker.layer = LayerMask.NameToLayer("Stickers");

        // Collider
        newSticker.AddComponent<BoxCollider2D>();

        // Posicion donde se hizo click
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0;
        newSticker.transform.position = worldPos;

        // Sprite
        var sr = newSticker.AddComponent<SpriteRenderer>();
        sr.sprite = sticker.sprite;
        sr.sortingOrder = 4;

        // Escala
        newSticker.transform.localScale = Vector3.one * 3f;

        // Gastar sticker
        StickerManager.Instance.UseSticker();
    }
}
}
