using UnityEngine;

public class CloudsMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Vector2 _direction = new Vector2(1, 1);

    [Header("Range of Speed")]
    [SerializeField] private float _speedMin = 3f;
    [SerializeField] private float _speedMax = 5f;

    private float _speed;

    void Start()
    {
        _speed = Random.Range(_speedMin, _speedMax);
        _direction = _direction.normalized;
    }

    void Update()
    {
        transform.Translate(_direction * _speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("CloudObstacle"))
        {
            Destroy(gameObject);
        }
    }
}
