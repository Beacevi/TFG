using UnityEngine;

public class StickerReceiver : MonoBehaviour
{
    private void OnMouseDown()
    {
        PlaceSticker();
    }

    void PlaceSticker()
    {
        if(StickerManager.Instance != null)
        {
            var sticker = StickerManager.Instance.selectedSticker;


            if (sticker == null || sticker.amount <= 0)
            {
                return;
            }
                

            // Crear objeto sticker
            GameObject newSticker = new GameObject("Sticker");
            newSticker.transform.SetParent(transform);

            // Posicion donde hiciste click
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = 0;
            newSticker.transform.position = worldPos;

            // Añadir sprite
            var sr = newSticker.AddComponent<SpriteRenderer>();
            sr.sprite = sticker.sprite;

            sr.sortingOrder = 4;

            // Opcional: ajustar tamaño
            newSticker.transform.localScale = Vector3.one * 0.5f;

            // Gastar sticker
            StickerManager.Instance.UseSticker();
        }
    }
}