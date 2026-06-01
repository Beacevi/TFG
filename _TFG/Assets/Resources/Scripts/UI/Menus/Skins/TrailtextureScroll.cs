/**
 * @file TrailtextureScroll.cs
 * @brief Desplaza una textura o rastro visual creando un efecto de movimiento curvo.
 * @author Hortensia Studio - Beatriz Ceballos Vidal
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using UnityEngine;

/// <summary>
/// Desplaza una textura o rastro visual creando un efecto de movimiento curvo.
/// </summary>
public class TrailCurvedMotion : MonoBehaviour
{
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar speed x.
    /// </summary>
    public float speedX = 2f;          // velocidad horizontal
    /// <summary>
    /// Valor numérico que limita o define max distance.
    /// </summary>
    public float maxDistance = 5f;     // distancia antes de reiniciar
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar curve amplitude.
    /// </summary>
    public float curveAmplitude = 0.5f; // altura de la curva
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar curve frequency.
    /// </summary>
    public float curveFrequency = 1f;   // frecuencia de la curva

    /// <summary>
    /// Campo de tipo Vector3 utilizado para almacenar o configurar start pos.
    /// </summary>
    private Vector3 startPos;

    /// <summary>
    /// Inicializa el componente cuando la escena ya está cargada y lista para comenzar.
    /// </summary>
    void Start()
    {
        startPos = transform.position;
    }

    /// <summary>
    /// Actualiza la lógica del componente en cada fotograma.
    /// </summary>
    void Update()
    {
        // 1️⃣ Mover horizontalmente
        transform.position += Vector3.right * speedX * Time.deltaTime;

        // 2️⃣ Reiniciar X si se pasa del límite
        if (transform.position.x > startPos.x + maxDistance)
        {
            transform.position = new Vector3(startPos.x, startPos.y, startPos.z);
        }

        // 3️⃣ Aplicar curvatura vertical (sin tocar X)
        float offsetY = Mathf.Sin(Time.time * curveFrequency) * curveAmplitude;
        transform.position = new Vector3(transform.position.x, startPos.y + offsetY, transform.position.z);
    }
}
