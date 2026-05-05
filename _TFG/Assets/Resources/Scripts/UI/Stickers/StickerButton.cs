using TMPro;
using UnityEngine;

public class StickerButton : MonoBehaviour
{
    public StickerData sticker;
    public TextMeshProUGUI cantidadTexto;

    public void OnClick()
    {
        StickerManager.Instance.SelectSticker(sticker);
    }
}