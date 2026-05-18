using UnityEngine;

public class StickerReceiver : MonoBehaviour
{
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