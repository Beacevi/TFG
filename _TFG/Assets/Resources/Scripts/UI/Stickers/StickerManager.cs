using TMPro;
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

    //public void CargarRunas()
    //{
    //    var items = InventoryManager.instance.GetAvailableResources();

    //    for (int i = 0; i < inventarioRutas.transform.childCount; i++)
    //    {
    //        inventarioRutas.transform.GetChild(i).gameObject.SetActive(false);
    //    }

    //    for (int i = 0; i < items.Count; i++)
    //    {
    //        if (i >= inventarioRutas.transform.childCount)
    //            break;

    //        Transform boton = inventarioRutas.transform.GetChild(i);
    //        boton.gameObject.SetActive(true);

    //        InventoryItem item = items[i];

    //        // icono y texto
    //        Image img = boton.GetComponentInChildren<Image>();
    //        img.sprite = item.resource.displaySprite;

    //        TMP_Text txt = boton.GetComponentInChildren<TMP_Text>();
    //        txt.text = item.amount.ToString();

    //        //AQUÍ ESTÁ LA CLAVE
    //        RuneButton rune = boton.GetComponent<RuneButton>();
    //        rune.Init(item.resource);
    //    }
    //}

}