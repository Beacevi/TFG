/**
 * @file Sounds.cs
 * @brief Gestiona la reproducción de efectos de sonido y música asociados a la interfaz o al juego.
 * @author Hortensia Studio - Beatriz Ceballos Vidal
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
    /// Sonido reproducido cuando el jugador hace clic/toque en el mapa procedural para moverse.
    /// </summary>
    public AudioClip sonidoClickMapa;

    /// <summary>
    /// Sonido reproducido cada vez que el jugador llega a una casilla durante el desplazamiento.
    /// </summary>
    public AudioClip sonidoPasos;

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

    /// <summary>
    /// Reproduce el sonido de clic/toque sobre el mapa procedural.
    /// Usa PlayOneShot para no interrumpir el resto de audio en curso.
    /// </summary>
    public void SonidoClickMapa()
    {
        if (src != null && sonidoClickMapa != null)
        {
            src.PlayOneShot(sonidoClickMapa);
        }
    }

    /// <summary>
    /// Reproduce el sonido de paso del jugador al alcanzar una casilla.
    /// Usa PlayOneShot para que varios pasos consecutivos no se interrumpan entre sí.
    /// </summary>
    public void SonidoPasos()
    {
        if (src != null && sonidoPasos != null)
        {
            src.PlayOneShot(sonidoPasos);
        }
    }



}
