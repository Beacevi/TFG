/**
 * @file Coin.cs
 * @brief Representa una moneda interactiva dentro del escenario y su comportamiento de recogida.
 * @author Hortensia Studio - Alejandro Romero Burgada
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using UnityEngine;

/// <summary>
/// Representa una moneda interactiva dentro del escenario y su comportamiento de recogida.
/// </summary>
[System.Serializable]
public class Coin
{
    /// <summary>
    /// Campo de tipo string utilizado para almacenar o configurar name.
    /// </summary>
    public string name;
    /// <summary>
    /// Prefab o conjunto de prefabs utilizados para instanciar coin prefab.
    /// </summary>
    public GameObject coinPrefab;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar spawn chance.
    /// </summary>
    [Range(0f, 1f)] public float spawnChance = 1f;
}
