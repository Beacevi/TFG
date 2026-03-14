using UnityEngine;

public class TrailCurvedMotion : MonoBehaviour
{
    public float speedX = 2f;          // velocidad horizontal
    public float maxDistance = 5f;     // distancia antes de reiniciar
    public float curveAmplitude = 0.5f; // altura de la curva
    public float curveFrequency = 1f;   // frecuencia de la curva

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

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
