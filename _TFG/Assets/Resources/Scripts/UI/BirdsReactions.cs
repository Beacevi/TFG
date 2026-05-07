using System.Collections;
using UnityEngine;

public class BirdsReactions : MonoBehaviour
{
    public BirdReactionsManager manager;

    [Header("Bird Data")]
    public Bird birdData;

    private AudioSource src;
    private Vector3 originalPos;

    void Awake()
    {
        src = GetComponent<AudioSource>();
        originalPos = transform.position;
    }

    void OnMouseDown()
    {
        if (manager == null)
        {
            Debug.LogWarning("BirdReactionsManager no asignado en " + gameObject.name);
            return;
        }

        manager.TriggerReaction(this);
    }

    public void PlaySound()
    {
        if (src == null)
        {
            Debug.LogWarning("AudioSource falta en " + gameObject.name);
            return;
        }

        GameManager.Instance.GetComponent<Sounds>().SonidoTocarPajaro(src); //Sustituir por birdData.PlaySound(); o algo
    }

    public void PlayMovement()
    {
        if (birdData == null)
        {
            Debug.LogWarning("BirdData es null en " + gameObject.name);
            return;
        }

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
        }
    }

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

    IEnumerator JumpRoutine()
    {
        Vector3 target = originalPos + Vector3.up * birdData.moveRange;

        yield return MoveTo(target);
        yield return MoveTo(originalPos);
    }

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