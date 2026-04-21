using UnityEngine;

public class StickerButton : MonoBehaviour
{
    public StickerData sticker;

    public void OnClick()
    {
        StickerManager.Instance.SelectSticker(sticker);
    }
}