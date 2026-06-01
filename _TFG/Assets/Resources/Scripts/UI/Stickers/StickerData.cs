/**
 * @file StickerData.cs
 * @brief Modelo de datos que describe un sticker o elemento decorativo.
 * @author Hortensia Studio - David Díaz Espinosa de los Monteros
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using TMPro;
using UnityEngine;

/// <summary>
/// Modelo de datos que describe un sticker o elemento decorativo.
/// </summary>
[CreateAssetMenu(fileName = "NewSticker", menuName = "Stickers/Sticker")]
public class StickerData : ScriptableObject
{
    /// <summary>
    /// Campo de tipo string utilizado para almacenar o configurar sticker name.
    /// </summary>
    public string stickerName;

    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar id.
    /// </summary>
    public int id;

    /// <summary>
    /// Referencia visual o sprite asociado a sprite.
    /// </summary>
    public Sprite sprite;
    /// <summary>
    /// Referencia visual o sprite asociado a unknown sprite.
    /// </summary>
    public Sprite unknownSprite;

    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar amount.
    /// </summary>
    public int amount = 0;
    /// <summary>
    /// Indica si discovered está activo o habilitado.
    /// </summary>
    public bool discovered = false;

    /// <summary>
    /// Obtiene display sprite a partir del estado actual del sistema.
    /// </summary>
    /// <returns>Instancia o valor de tipo Sprite resultante de la operación.</returns>
    public Sprite GetDisplaySprite()
    {
        return discovered ? sprite : unknownSprite;
    }

}
