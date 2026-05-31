/**
 * @file BirdsReactions.cs
 * @brief Define y ejecuta reacciones individuales asociadas a las aves.
 * @author Hortensia Studio
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using System.Collections;
using UnityEngine;

/// <summary>
/// Define y ejecuta reacciones individuales asociadas a las aves.
/// </summary>
public class BirdsReactions : MonoBehaviour
{
    /// <summary>
    /// Referencia al gestor encargado de manager.
    /// </summary>
    public BirdReactionsManager manager;

    /// <summary>
    /// Campo de tipo Bird utilizado para almacenar o configurar bird data.
    /// </summary>
    [Header("Bird Data")]
    public Bird birdData;

    /// <summary>
    /// Campo de tipo AudioSource utilizado para almacenar o configurar src.
    /// </summary>
    private AudioSource src;
    /// <summary>
    /// Campo de tipo Vector3 utilizado para almacenar o configurar original pos.
    /// </summary>
    private Vector3 originalPos;

    /// <summary>
    /// Inicializa referencias internas antes de que comience la ejecución normal del componente.
    /// </summary>
    void Awake()
    {
        src = GetComponent<AudioSource>();
        originalPos = transform.position;
    }

    /// <summary>
    /// Procesa la pulsación directa sobre el objeto desde el ratón o entrada equivalente.
    /// </summary>
    void OnMouseDown()
    {
        if (manager == null)
        {
            Debug.LogWarning("BirdReactionsManager no asignado en " + gameObject.name);
            return;
        }

        manager.TriggerReaction(this);
    }

    /// <summary>
    /// Reproduce o inicia sound.
    /// </summary>
    public void PlaySound()
    {
        if (src == null)
        {
            Debug.LogWarning("AudioSource falta en " + gameObject.name);
            return;
        }

        GameManager.Instance.GetComponent<Sounds>().SonidoTocarPajaro(src); //Sustituir por birdData.PlaySound(); o algo
    }

    /// <summary>
    /// Reproduce o inicia movement.
    /// </summary>
    public void PlayMovement()
    {
        if (birdData == null)
        {
            Debug.LogWarning("BirdData es null en " + gameObject.name);
            return;
        }
        /*
        switch (birdData.movementType)
        {
            case BirdMovementType.Random:
                StartCoroutine(RandomMoveRoutine());
                break;

            case BirdMovementType.Jump:
                StartCoroutine(JumpRoutine());
                break;

            case BirdMovementType.Shake:
                StartCoroutine(ShakeRoutine());
                break;

            case BirdMovementType.Circle:
                StartCoroutine(CircleRoutine());
                break;
        }*/
    }

    /// <summary>
    /// Ejecuta la lógica asociada a random move routine dentro de BirdsReactions.
    /// </summary>
    /// <returns>Corrutina que permite ejecutar la operación de forma diferida en Unity.</returns>
    IEnumerator RandomMoveRoutine()
    {
        Vector2 offset = new Vector2(
            Random.Range(-birdData.moveRange, birdData.moveRange),
            Random.Range(-birdData.moveRange, birdData.moveRange)
        );

        Vector3 target = originalPos + (Vector3)offset;

        yield return MoveTo(target);
        yield return MoveTo(originalPos);
    }

    /// <summary>
    /// Ejecuta la lógica asociada a jump routine dentro de BirdsReactions.
    /// </summary>
    /// <returns>Corrutina que permite ejecutar la operación de forma diferida en Unity.</returns>
    IEnumerator JumpRoutine()
    {
        Vector3 target = originalPos + Vector3.up * birdData.moveRange;

        yield return MoveTo(target);
        yield return MoveTo(originalPos);
    }

    /// <summary>
    /// Ejecuta la lógica asociada a shake routine dentro de BirdsReactions.
    /// </summary>
    /// <returns>Corrutina que permite ejecutar la operación de forma diferida en Unity.</returns>
    IEnumerator ShakeRoutine()
    {
        float timer = 0;

        while (timer < birdData.moveDuration)
        {
            timer += Time.deltaTime;

            Vector2 offset = Random.insideUnitCircle * birdData.moveRange;

            transform.position = originalPos + (Vector3)offset;

            yield return null;
        }

        transform.position = originalPos;
    }

    /// <summary>
    /// Ejecuta la lógica asociada a circle routine dentro de BirdsReactions.
    /// </summary>
    /// <returns>Corrutina que permite ejecutar la operación de forma diferida en Unity.</returns>
    IEnumerator CircleRoutine()
    {
        float timer = 0;

        while (timer < birdData.moveDuration)
        {
            timer += Time.deltaTime;

            float angle = timer * 360f;

            Vector3 offset = new Vector3(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad),
                0
            ) * birdData.moveRange;

            transform.position = originalPos + offset;

            yield return null;
        }

        transform.position = originalPos;
    }

    /// <summary>
    /// Ejecuta la lógica asociada a move to dentro de BirdsReactions.
    /// </summary>
    /// <param name="target">Parámetro target empleado por el método.</param>
    /// <returns>Corrutina que permite ejecutar la operación de forma diferida en Unity.</returns>
    IEnumerator MoveTo(Vector3 target)
    {
        Vector3 start = transform.position;
        float t = 0;

        while (t < 1f)
        {
            t += Time.deltaTime / birdData.moveDuration;
            transform.position = Vector3.Lerp(start, target, t);
            yield return null;
        }
    }
}
