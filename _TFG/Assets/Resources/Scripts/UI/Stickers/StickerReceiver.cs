using UnityEngine;

public class StickerReceiver : MonoBehaviour
{
    private void OnMouseDown()
    {
        PlaceSticker();
    }

    void PlaceSticker()
    {
        var sticker = StickerManager.Instance.selectedSticker;

        if (sticker == null || sticker.amount <= 0)
            return;

        // Crear objeto sticker
        GameObject stickerGO = new GameObject("Sticker");
        stickerGO.transform.SetParent(transform);

        // Posición donde hiciste click
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0;
        stickerGO.transform.position = worldPos;

        // Añadir sprite
        var sr = stickerGO.AddComponent<SpriteRenderer>();
        sr.sprite = sticker.sprite;

        sr.sortingOrder = 4;

        // Opcional: ajustar tamaño
        stickerGO.transform.localScale = Vector3.one * 0.5f;

        // Gastar sticker
        StickerManager.Instance.UseSticker();
    }
}