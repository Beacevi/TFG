using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StickerManager : MonoBehaviour
{
    public static StickerManager Instance;

    public StickerData selectedSticker;

    public List<GameObject>listaSticker;
    public bool modoActual = false;//False es colocar, true es borrar

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

        for (int i = 0; i < listaSticker.Count; i++)
        {
            GameObject boton = listaSticker[i];

            StickerUIButton datosBoton = boton.GetComponent<StickerUIButton>();

            StickerData sticker = datosBoton.sticker;

            if (selectedSticker != null && sticker.id == selectedSticker.id)
            {
                datosBoton.cantidadTexto.text = sticker.amount.ToString();
            }

        }
        selectedSticker = null;

    }

    public void CambiarModo()
    {
        modoActual = !modoActual;
    }

    public void CargarStickers()
    {
        for (int i = 0; i < listaSticker.Count; i++)
        {

            GameObject boton = listaSticker[i];

            StickerUIButton datosBoton = boton.GetComponent<StickerUIButton>();

            StickerData sticker = datosBoton.sticker;

            if (sticker.discovered)
            {
                listaSticker[i].SetActive(true);
                datosBoton.cantidadTexto.enabled = true;
                datosBoton.cantidadTexto.text = sticker.amount.ToString();
                boton.GetComponent<Image>().sprite = sticker.sprite;
            }
            else
            {
                listaSticker[i].SetActive(true);
                boton.GetComponent<Image>().sprite = sticker.unknownSprite;
                datosBoton.cantidadTexto.enabled = false;
                //listaSticker[i].SetActive(false);
            }

            

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

    //        //AQU� EST� LA CLAVE
    //        RuneButton rune = boton.GetComponent<RuneButton>();
    //        rune.Init(item.resource);
    //    }
    //}

}