/**
 * @file CloudsMovement.cs
 * @brief Controla el desplazamiento continuo de una nube u objeto de fondo para reforzar la sensación de movimiento en la escena.
 * @author Hortensia Studio
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using UnityEngine;

/// <summary>
/// Controla el desplazamiento continuo de una nube u objeto de fondo para reforzar la sensación de movimiento en la escena.
/// </summary>
public class CloudsMovement : MonoBehaviour
{
    /// <summary>
    /// Campo de tipo Vector2 utilizado para almacenar o configurar direction.
    /// </summary>
    [Header("Movement")]
    [SerializeField] private Vector2 _direction = new Vector2(1, 1);

    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar speed min.
    /// </summary>
    [Header("Range of Speed")]
    [SerializeField] private float _speedMin = 3f;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar speed max.
    /// </summary>
    [SerializeField] private float _speedMax = 5f;

    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar speed.
    /// </summary>
    private float _speed;

    /// <summary>
    /// Inicializa el componente cuando la escena ya está cargada y lista para comenzar.
    /// </summary>
    void Start()
    {
        _speed = Random.Range(_speedMin, _speedMax);
        _direction = _direction.normalized;
    }

    /// <summary>
    /// Actualiza la lógica del componente en cada fotograma.
    /// </summary>
    void Update()
    {
        transform.Translate(_direction * _speed * Time.deltaTime);
    }

    /// <summary>
    /// Procesa la entrada de otro collider 2D en el área de detección.
    /// </summary>
    /// <param name="other">Collider u objeto que ha activado el evento.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("CloudObstacle"))
        {
            Destroy(gameObject);
        }
    }
}
