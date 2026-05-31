/**
 * @file ChangeCustomMenu.cs
 * @brief Gestiona el cambio de categoría o vista dentro del menú de personalización.
 * @author Hortensia Studio
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using UnityEngine;

/// <summary>
/// Gestiona el cambio de categoría o vista dentro del menú de personalización.
/// </summary>
public class ChangeCustomMenu : MonoBehaviour
{
    /// <summary>
    /// Color utilizado para representar custom menu colors.
    /// </summary>
    public GameObject customMenuColors;
    /// <summary>
    /// Campo de tipo GameObject utilizado para almacenar o configurar custom menu skins.
    /// </summary>
    public GameObject customMenuSkins;

    /// <summary>
    /// Ejecuta la lógica asociada a activar menu colors dentro de ChangeCustomMenu.
    /// </summary>
    public void ActivarMenuColors()
    {
        customMenuColors.SetActive(true);
        customMenuSkins.SetActive(false);
    }

    /// <summary>
    /// Ejecuta la lógica asociada a activar menu skins dentro de ChangeCustomMenu.
    /// </summary>
    public void ActivarMenuSkins()
    {
        customMenuColors.SetActive(false);
        customMenuSkins.SetActive(true);
    }

}
