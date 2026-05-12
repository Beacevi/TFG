using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StickerUIButton : MonoBehaviour
{
    public StickerData sticker;
    public TextMeshProUGUI cantidadTexto;

    public void OnClick()
    {
        if (sticker.discovered)
        {
            StickerManager.Instance.SelectSticker(sticker);
        }
    }

    public void Init(StickerData s)
    {
        /*sticker = s;

        Button button = GetComponent<Button>();
        button.onClick.RemoveAllListeners();

        button.onClick.AddListener(() =>
        {
            StickerManager.Instance.SelectSticker(sticker);
        });*/
    }
}