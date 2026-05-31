/**
 * @file Node.cs
 * @brief Representa una celda lógica del mapa procedural, incluyendo posición, tipo de terreno y referencias visuales.
 * @author Hortensia Studio
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using UnityEngine;

/// <summary>
/// Representa una celda lógica del mapa procedural, incluyendo posición, tipo de terreno y referencias visuales.
/// </summary>
public class Node
{
    /// <summary>
    /// Campo de tipo Vector2Int utilizado para almacenar o configurar position.
    /// </summary>
    public Vector2Int position;
    /// <summary>
    /// Indica si walkable está activo o habilitado.
    /// </summary>
    public bool walkable;
    /// <summary>
    /// Indica si has object está activo o habilitado.
    /// </summary>
    public bool hasObject;

    /// <summary>
    /// Campo de tipo GameObject utilizado para almacenar o configurar interactable.
    /// </summary>
    public GameObject Interactable;

    /// <summary>
    /// Campo de tipo Node utilizado para almacenar o configurar parent.
    /// </summary>
    public Node parent;
    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar g cost.
    /// </summary>
    public int gCost;
    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar h cost.
    /// </summary>
    public int hCost;
    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar f cost.
    /// </summary>
    public int fCost => gCost + hCost;

    /// <summary>
    /// Ejecuta la lógica asociada a node dentro de Node.
    /// </summary>
    /// <param name="pos">Posición o coordenada utilizada en el cálculo.</param>
    /// <param name="walkable">Parámetro walkable empleado por el método.</param>
    public Node(Vector2Int pos, bool walkable)
    {
        this.position = pos;
        this.walkable = walkable;
    }
}
