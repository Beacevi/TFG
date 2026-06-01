/**
 * @file BirdReactionsManager.cs
 * @brief Gestiona las reacciones visuales o expresivas de las aves ante eventos de interfaz.
 * @author Hortensia Studio - Beatriz Ceballos Vidal
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using UnityEngine;

/// <summary>
/// Gestiona las reacciones visuales o expresivas de las aves ante eventos de interfaz.
/// </summary>
public class BirdReactionsManager : MonoBehaviour
{
    /// <summary>
    /// Ejecuta la lógica asociada a trigger reaction dentro de BirdReactionsManager.
    /// </summary>
    /// <param name="bird">Parámetro bird empleado por el método.</param>
    public void TriggerReaction(BirdsReactions bird)
    {
        if (bird == null)
        {
            Debug.LogWarning("Bird es null en TriggerReaction");
            return;
        }

        int r = 0;

        switch (r)
        {
            case 0:
                Debug.Log("Se ha emitido un sonido");
                bird.PlaySound();
                break;
        }
    }
}
