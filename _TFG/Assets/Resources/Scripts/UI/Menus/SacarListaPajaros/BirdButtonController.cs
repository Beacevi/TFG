/**
 * @file BirdButtonController.cs
 * @brief Controla los botones generados dinámicamente para listar y seleccionar aves.
 * @author Hortensia Studio
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controla los botones generados dinámicamente para listar y seleccionar aves.
/// </summary>
public class BirdButtonController : MonoBehaviour
{
    /// <summary>
    /// Colección de birds utilizada por este componente.
    /// </summary>
    [Header("Birds y botones")]
    public Bird[] birds;      // Todos tus ScriptableObjects Bird
    /// <summary>
    /// Colección de botones utilizada por este componente.
    /// </summary>
    public Button[] botones;  // Botones de la UI, uno por Bird

    // Este método se llama al pulsar el botón de comprobación
    /// <summary>
    /// Ejecuta la lógica asociada a actualizar botones dentro de BirdButtonController.
    /// </summary>
    public void ActualizarBotones()
    {
        for (int i = 0; i < botones.Length; i++)
        {
            if (i < birds.Length)
            {
                // Activar o desactivar el botón según conseguido
                botones[i].gameObject.SetActive(birds[i].obtenido);
            }
            else
            {
                // Si hay más botones que birds, desactivamos extras
                botones[i].gameObject.SetActive(false);
            }
        }
    }
}
