using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using System.Collections;

public class IsometricCamera : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset = new Vector3(25, 25, -25);
    [SerializeField] float followSpeed = 5f;

    [SerializeField] ChangeScene changeScene;
    private Vector3 startPos;
    bool noMoreSteps = false;
    float distanciaMinima = 0.1f;

    private TileAStar tileAstar;

    enum CameraState
    {
        Following,
        Waiting,
        Returning,
        ChangingScene
    }

    CameraState state = CameraState.Following;

    void Start()
    {
        Camera cam = GetComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 10f;
        startPos = transform.position;
    }



    void LateUpdate()
    {
        if (tileAstar == null) return;

        switch (state)
        {
            case CameraState.Following:
                FollowPlayer();
                break;

            case CameraState.Returning:
                ReturnToStart();
                break;
        }
    }


    void FollowPlayer()
    {
        if (target == null) return;

        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            followSpeed * Time.deltaTime
        );

        if (tileAstar.stepsAvailable <= 0)
        {
            target = null;
            state = CameraState.Waiting;
            StartCoroutine(WaitBeforeReturn());
        }
    }

    IEnumerator WaitBeforeReturn()
    {
        yield return new WaitForSeconds(1f);
        state = CameraState.Returning;
    }

    void ReturnToStart()
    {
        transform.position = Vector3.Lerp(
            transform.position,
            startPos,
            followSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, startPos) < distanciaMinima)
        {
            state = CameraState.ChangingScene;
            changeScene.Cambiar_A_Escena("UI");
        }
    }


    public void AssingPlayer(GameObject _player)
    {
        target = _player.transform;
        tileAstar = target.gameObject.GetComponent<TileAStar>();
    }
}
