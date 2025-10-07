using UnityEngine;

public class CloudsMovement : MonoBehaviour
{
    [SerializeField] private float   _speed;
    [SerializeField] private Vector2 _direction = new Vector2(1, 1); //> Diagonal por defecto (45°)

    [Header("Range of Speed")]
    [SerializeField] private float _speedMin = 3f;
    [SerializeField] private float _speedMax = 5f;

    void Start()
    {
        _speed     = Random.Range(_speedMin, _speedMax);
        _direction = _direction.normalized; //> Normaliza la dirección para mantener la misma velocidad en cualquier ángulo
    }

    void Update()
    {
        transform.Translate(_direction * _speed * Time.deltaTime); //> Mueve el objeto en la dirección elegida
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("CloudObstacle"))
        {
            Destroy(gameObject);
        }
    }
}
