/**
 * @file CoinClick.cs
 * @brief Detecta la interacción del jugador con una moneda y aplica la lógica asociada a su recogida.
 * @author Hortensia Studio - Beatriz Ceballos Vidal
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using System.Collections;
using UnityEngine;

/// <summary>
/// Detecta la interacción del jugador con una moneda y aplica la lógica asociada a su recogida.
/// </summary>
public class CoinClick : MonoBehaviour
{
    /// <summary>
    /// Campo de tipo AudioSource utilizado para almacenar o configurar src.
    /// </summary>
    public AudioSource src;
    /// <summary>
    /// Campo de tipo AudioClip utilizado para almacenar o configurar sound.
    /// </summary>
    public AudioClip sound;
    /// <summary>
    /// Campo de tipo CircleCollider2D utilizado para almacenar o configurar collider coin.
    /// </summary>
    public CircleCollider2D colliderCoin;
    /// <summary>
    /// Referencia visual o sprite asociado a sprite coin.
    /// </summary>
    public SpriteRenderer spriteCoin;
    /// <summary>
    /// Campo de tipo ParticleSystem utilizado para almacenar o configurar shiny particle.
    /// </summary>
    public ParticleSystem shinyParticle;
    
    /// <summary>
    /// Procesa la pulsación directa sobre el objeto desde el ratón o entrada equivalente.
    /// </summary>
    private void OnMouseDown()
    {
        GameManager.Instance.AddMoney(1);
        src.clip = sound;
        src.Play();
        StartCoroutine(DestroyObject());
        //Particulas
        
    }

    /// <summary>
    /// Ejecuta la lógica asociada a destroy object dentro de CoinClick.
    /// </summary>
    /// <returns>Corrutina que permite ejecutar la operación de forma diferida en Unity.</returns>
    private IEnumerator DestroyObject()
    {
        shinyParticle.Stop();
        colliderCoin.enabled = false;
        spriteCoin.enabled = false;
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

}
