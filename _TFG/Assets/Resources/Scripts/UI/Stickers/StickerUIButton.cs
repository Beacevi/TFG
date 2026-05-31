/**
 * @file StickerUIButton.cs
 * @brief Botón de interfaz que permite seleccionar o instanciar un sticker concreto.
 * @author Hortensia Studio
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Botón de interfaz que permite seleccionar o instanciar un sticker concreto.
/// </summary>
public class StickerUIButton : MonoBehaviour
{
    /// <summary>
    /// Campo de tipo StickerData utilizado para almacenar o configurar sticker.
    /// </summary>
    public StickerData sticker;
    /// <summary>
    /// Campo de tipo TextMeshProUGUI utilizado para almacenar o configurar cantidad texto.
    /// </summary>
    public TextMeshProUGUI cantidadTexto;

    /// <summary>
    /// Ejecuta la lógica asociada a on click dentro de StickerUIButton.
    /// </summary>
    public void OnClick()
    {
        if (sticker.discovered)
        {
            StickerManager.Instance.SelectSticker(sticker);
        }
    }

    /// <summary>
    /// Ejecuta la lógica asociada a init dentro de StickerUIButton.
    /// </summary>
    /// <param name="s">Parámetro s empleado por el método.</param>
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
