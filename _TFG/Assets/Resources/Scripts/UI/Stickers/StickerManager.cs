/**
 * @file StickerManager.cs
 * @brief Gestiona la creación, selección y organización de stickers dentro del sistema de personalización.
 * @author Hortensia Studio - David Díaz Espinosa de los Monteros
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gestiona la creación, selección y organización de stickers dentro del sistema de personalización.
/// </summary>
public class StickerManager : MonoBehaviour
{
    /// <summary>
    /// Instancia singleton utilizada para acceder globalmente al controlador.
    /// </summary>
    public static StickerManager Instance;

    /// <summary>
    /// Campo de tipo StickerData utilizado para almacenar o configurar selected sticker.
    /// </summary>
    public StickerData selectedSticker;

    public List<GameObject>listaSticker;
    /// <summary>
    /// Indica si modo actual está activo o habilitado.
    /// </summary>
    public bool modoActual = false;//False es colocar, true es borrar

    /// <summary>
    /// Inicializa referencias internas antes de que comience la ejecución normal del componente.
    /// </summary>
    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Ejecuta la lógica asociada a select sticker dentro de StickerManager.
    /// </summary>
    /// <param name="sticker">Parámetro sticker empleado por el método.</param>
    public void SelectSticker(StickerData sticker)
    {
        if (sticker.amount > 0)
        {
            selectedSticker = sticker;
        }
    }

    /// <summary>
    /// Ejecuta la lógica asociada a use sticker dentro de StickerManager.
    /// </summary>
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

    /// <summary>
    /// Cambia cambiar modo según la interacción o parámetro recibido.
    /// </summary>
    public void CambiarModo()
    {
        modoActual = !modoActual;
    }

    /// <summary>
    /// Ejecuta la lógica asociada a cargar stickers dentro de StickerManager.
    /// </summary>
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
}
