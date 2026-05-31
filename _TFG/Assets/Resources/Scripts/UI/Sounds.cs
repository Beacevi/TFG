/**
 * @file Sounds.cs
 * @brief Gestiona la reproducción de efectos de sonido y música asociados a la interfaz o al juego.
 * @author Hortensia Studio
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using UnityEngine;

/// <summary>
/// Gestiona la reproducción de efectos de sonido y música asociados a la interfaz o al juego.
/// </summary>
public class Sounds : MonoBehaviour
{
    /// <summary>
    /// Campo de tipo AudioSource utilizado para almacenar o configurar src.
    /// </summary>
    public AudioSource src;
    /// <summary>
    /// Campo de tipo AudioClip utilizado para almacenar o configurar sonido interfaz boton.
    /// </summary>
    public AudioClip sonidoInterfazBoton;
    /// <summary>
    /// Campo de tipo AudioClip utilizado para almacenar o configurar sonido recolectar pajaro.
    /// </summary>
    public AudioClip sonidoRecolectarPajaro;
    /// <summary>
    /// Campo de tipo AudioClip utilizado para almacenar o configurar sonido alinear pajaros.
    /// </summary>
    public AudioClip sonidoAlinearPajaros;

    /// <summary>
    /// Campo de tipo AudioClip utilizado para almacenar o configurar sonido tocar pajaro.
    /// </summary>
    public AudioClip sonidoTocarPajaro;

    /// <summary>
    /// Inicializa el componente cuando la escena ya está cargada y lista para comenzar.
    /// </summary>
    public void Start()
    {
        src = GameManager.Instance.GetComponent<AudioSource>();
    }
    /// <summary>
    /// Ejecuta la lógica asociada a sonido interfaz boton dentro de Sounds.
    /// </summary>
    public void SonidoInterfazBoton()
    {
        src.clip = sonidoInterfazBoton;
        src.Play();
    }
    /// <summary>
    /// Ejecuta la lógica asociada a sonido recolectar pajaro dentro de Sounds.
    /// </summary>
    public void SonidoRecolectarPajaro()
    {
        src.clip = sonidoRecolectarPajaro;
        src.Play();
    }

    /// <summary>
    /// Ejecuta la lógica asociada a sonido alinear pajaros dentro de Sounds.
    /// </summary>
    /// <param name="source">Parámetro source empleado por el método.</param>
    public void SonidoAlinearPajaros(AudioSource source)
    {
        source.clip = sonidoAlinearPajaros;
        source.Play();
    }

    /// <summary>
    /// Ejecuta la lógica asociada a sonido tocar pajaro dentro de Sounds.
    /// </summary>
    /// <param name="source">Parámetro source empleado por el método.</param>
    public void SonidoTocarPajaro(AudioSource source)
    {
        source.clip = sonidoTocarPajaro;
        source.Play();
    }



}
