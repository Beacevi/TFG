using UnityEngine;

public class IsometricCamera : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(25, 25, -25);
    public float followSpeed = 5f;

    void Start()
    {
        Camera cam = GetComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 10f;
    }

    void LateUpdate()
    {
        if (player != null)
        {
            Vector3 targetPosition = player.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
    }

    public void AssingPlayer(GameObject _player)
    {
        player = _player.transform;
    }
}
