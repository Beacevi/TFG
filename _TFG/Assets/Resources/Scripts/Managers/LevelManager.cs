/**
 * @file LevelManager.cs
 * @brief Gestiona información de nivel y progresión usada por los sistemas de juego.
 * @author Hortensia Studio
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using UnityEngine;

/// <summary>
/// Gestiona información de nivel y progresión usada por los sistemas de juego.
/// </summary>
public class LevelManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    /// <summary>
    /// Inicializa el componente cuando la escena ya está cargada y lista para comenzar.
    /// </summary>
    void Start()
    {
        
    }

    // Update is called once per frame
    /// <summary>
    /// Actualiza la lógica del componente en cada fotograma.
    /// </summary>
    void Update()
    {
        
    }

    /// <summary>
    /// Ejecuta la lógica asociada a upgrade level dentro de LevelManager.
    /// </summary>
    public void UpgradeLevel()
    {
        // L�gica para subir de nivel

        GameManager.Instance.AddMoney(-200); // Insertar valor del CSV
    }
}
