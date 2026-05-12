using UnityEngine;

public class BirdIdleMovement : MonoBehaviour
{
    public Bird birdData;

    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        if (birdData == null)
        {
            return;
        } 

        switch (birdData.idleMovementType)
        {
            case BirdIdleMovementType.Float:
                Float();
                break;

            case BirdIdleMovementType.Hover:
                Hover();
                break;

            case BirdIdleMovementType.Shake:
                Shake();
                break;

            case BirdIdleMovementType.Circle:
                Circle();
                break;

            case BirdIdleMovementType.Sway:
                Sway();
                break;
        }
    }

    void Float()
    {
        float y = Mathf.Sin(Time.time * birdData.idleSpeed) * birdData.idleAmplitude;
        transform.position = startPos + new Vector3(0, y, 0);
    }

    void Hover()
    {
        float y = Mathf.Sin(Time.time * birdData.idleSpeed) * birdData.idleHoverOffset;
        transform.position = startPos + new Vector3(0, y, 0);
    }

    void Shake()
    {
        Vector2 offset = Random.insideUnitCircle * birdData.idleShakeIntensity;
        transform.position = startPos + new Vector3(offset.x, offset.y, 0);
    }

    void Circle()
    {
        float x = Mathf.Cos(Time.time * birdData.idleCircleSpeed) * birdData.idleCircleRadius;
        float y = Mathf.Sin(Time.time * birdData.idleCircleSpeed) * birdData.idleCircleRadius;

        transform.position = startPos + new Vector3(x, y, 0);
    }

    void Sway()
    {
        float x = Mathf.Sin(Time.time * birdData.idleSpeed) * birdData.idleAmplitude;
        transform.position = startPos + new Vector3(x, 0, 0);
    }
}