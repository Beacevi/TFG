/**
 * @file OnClickCustomSprite.cs
 * @brief Gestiona la selección de sprites o aspectos desde el menú de personalización.
 * @author Hortensia Studio - Beatriz Ceballos Vidal
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using UnityEngine;

/// <summary>
/// Gestiona la selección de sprites o aspectos desde el menú de personalización.
/// </summary>
public class OnClickCustomSprite : MonoBehaviour
{
    /// <summary>
    /// Referencia visual o sprite asociado a canvas custom sprites.
    /// </summary>
    [SerializeField] private CustomMenuSprites _canvasCustomSprites;

    // Cambiar el sprite hacia la izquierda
    /// <summary>
    /// Ejecuta la lógica asociada a on button click left change custom sprite dentro de OnClickCustomSprite.
    /// </summary>
    public void OnButtonClickLeftChangeCustomSprite()
    {
        string buttonTag = gameObject.tag;
        _canvasCustomSprites.ChangePart(buttonTag, true);
    }

    // Cambiar el sprite hacia la derecha
    /// <summary>
    /// Ejecuta la lógica asociada a on button click right change custom sprite dentro de OnClickCustomSprite.
    /// </summary>
    public void OnButtonClickRightChangeCustomSprite()
    {
        string buttonTag = gameObject.tag;
        _canvasCustomSprites.ChangePart(buttonTag, false);
    }
}
