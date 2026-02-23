using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlacedStickerUI : MonoBehaviour, IPointerClickHandler
{
    private StickerData stickerData;
    private StickerPlacer placer;

    public void Setup(StickerData data, StickerPlacer stickerPlacer)
    {
        stickerData = data;
        placer = stickerPlacer;

        GetComponent<Image>().sprite = data.sprite;
    }

    // Called when player taps the sticker
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!placer.IsDeleteMode())
            return;

        placer.DeleteSticker(this);
    }

    public StickerData GetData()
    {
        return stickerData;
    }
}