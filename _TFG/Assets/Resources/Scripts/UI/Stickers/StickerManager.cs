using UnityEngine;

public class StickerManager : MonoBehaviour
{
    public static StickerManager Instance;

    public StickerData selectedSticker;

    private void Awake()
    {
        Instance = this;
    }

    public void SelectSticker(StickerData sticker)
    {
        if (sticker.amount > 0)
        {
            selectedSticker = sticker;
        }
    }

    public void UseSticker()
    {
        if (selectedSticker != null && selectedSticker.amount > 0)
        {
            selectedSticker.amount--;
        }
    }
}