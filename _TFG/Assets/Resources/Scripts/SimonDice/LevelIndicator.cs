/**
 * @file LevelIndicator.cs
 * @brief Muestra el nivel o ronda actual dentro de la interfaz del minijuego.
 * @author Hortensia Studio - David Díaz Espinosa de los Monteros
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Muestra el nivel o ronda actual dentro de la interfaz del minijuego.
/// </summary>
public class LevelIndicator : MonoBehaviour
{
    /// <summary>
    /// Colección de dots utilizada por este componente.
    /// </summary>
    public SpriteRenderer[] dots;

    /// <summary>
    /// Establece o actualiza level success dentro del sistema.
    /// </summary>
    /// <param name="level">Nivel que se utilizará como referencia.</param>
    public void SetLevelSuccess(int level)
    {
        Color sucessColor = new Color32(103, 80, 164, 255);
        if (level < dots.Length)
            dots[level].color = sucessColor;
    }

    /// <summary>
    /// Establece o actualiza level fail dentro del sistema.
    /// </summary>
    /// <param name="level">Nivel que se utilizará como referencia.</param>
    public void SetLevelFail(int level)
    {
        Color failColor = new Color32(179, 38, 30, 255);
        if (level < dots.Length)
            dots[level].color = failColor;
    }

    /// <summary>
    /// Restablece los valores del sistema a su configuración inicial o por defecto.
    /// </summary>
    public void ResetIndicators()
    {
        Color resetColor = new Color32(232, 221, 255, 255);
        foreach (var dot in dots)
            dot.color = resetColor;
    }
}
