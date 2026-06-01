/**
 * @file SaveDataManager.cs
 * @brief Estructura serializable que agrupa los datos persistentes guardados en disco.
 * @author Hortensia Studio - David Díaz Espinosa de los Monteros
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Estructura serializable que agrupa los datos persistentes guardados en disco.
/// </summary>
[System.Serializable]
public class SaveDataManager
{
    /// <summary>
    /// Cantidad de monedas almacenadas o mostradas por el sistema.
    /// </summary>
    public int coins;
    /// <summary>
    /// Cantidad de gemas almacenadas o mostradas por el sistema.
    /// </summary>
    public int gems;
    /// <summary>
    /// Valor de energía o stamina disponible para el jugador.
    /// </summary>
    public int energy;
    /// <summary>
    /// Nivel actual de progreso del jugador.
    /// </summary>
    public int currentLevel;
    /// <summary>
    /// Nivel actual del globo aerostático.
    /// </summary>
    public int balloonLevel;

    /// <summary>
    /// Colección de shop items utilizada por este componente.
    /// </summary>
    public List<ColorItem> shopItems;
    /// <summary>
    /// Colección de unlocked colors utilizada por este componente.
    /// </summary>
    public List<Color32> unlockedColors;
}
