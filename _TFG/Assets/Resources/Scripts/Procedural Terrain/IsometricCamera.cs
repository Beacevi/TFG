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

    [SerializeField] private Animator animator;

    [Header("Camera Control")]
    [SerializeField] float dragSpeed = 0.01f;
    [SerializeField] float zoomSpeed = 0.1f;
    [SerializeField] float minZoom = 5f;
    [SerializeField] float maxZoom = 20f;
    [SerializeField] float maxDistanceFromTarget = 15f;

    private Camera cam;
    private bool isAttached = true;
    private Vector3 lastMousePos;
    private float initialZoom;

    private bool isDragging = false;
    private float dragThreshold = 10f;

    public static bool inputEnabled = true;

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
        animator.speed = -1;
        animator.Play("CloudsClosing", 0, 0f);

        cam = GetComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 10f;

        initialZoom = cam.orthographicSize;
        startPos = transform.position;
    }

    void LateUpdate()
    {
        if (tileAstar == null) return;

        HandleInput();

        switch (state)
        {
            case CameraState.Following:
                if (isAttached)
                    FollowPlayer();
                break;

            case CameraState.Returning:
                ReturnToStart();
                break;
        }
    }
    void HandleInput()
    {
        HandleMouse();
        HandleTouch();
    }

    void HandleMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastMousePos = Input.mousePosition;
            isDragging = false;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 delta = Input.mousePosition - lastMousePos;

            if (!isDragging && delta.magnitude > dragThreshold)
            {
                isDragging = true;
                isAttached = false;
                tileAstar.SetCanMove(false);
            }

            if (isDragging)
            {
                MoveCamera(delta);
            }

            lastMousePos = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (!isDragging)
            {
                tileAstar.SetCanMove(true);
                tileAstar.ProcessClick(Input.mousePosition);
            }
            else
            {
                tileAstar.SetCanMove(false);
            }
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            Zoom(scroll * 100f);
        }
    }

    void HandleTouch()
    {
        if (Input.touchCount == 1)
        {
            Touch t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Moved)
            {
                MoveCamera(t.deltaPosition);
                isAttached = false;
                tileAstar.SetCanMove(false);
            }
        }

        if (Input.touchCount == 2)
        {
            Touch t1 = Input.GetTouch(0);
            Touch t2 = Input.GetTouch(1);

            Vector2 prevT1 = t1.position - t1.deltaPosition;
            Vector2 prevT2 = t2.position - t2.deltaPosition;

            float prevDist = (prevT1 - prevT2).magnitude;
            float currentDist = (t1.position - t2.position).magnitude;

            float delta = currentDist - prevDist;

            Zoom(delta * zoomSpeed);
        }
    }

    void MoveCamera(Vector3 delta)
    {
        Vector3 move = new Vector3(-delta.x, -delta.y, 0) * dragSpeed;
        transform.Translate(move);

        ClampDistance();
    }

    void ClampDistance()
    {
        if (target == null) return;

        Vector3 offset = transform.position - target.position;

        if (offset.magnitude > maxDistanceFromTarget)
        {
            offset = offset.normalized * maxDistanceFromTarget;
            transform.position = target.position + offset;
        }
    }

    void Zoom(float increment)
    {
        cam.orthographicSize -= increment;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
    }

    public void ResetCamera()
    {
        isAttached = true;
        cam.orthographicSize = initialZoom;

        if (target != null)
        {
            transform.position = target.position + offset;
        }

        tileAstar.SetCanMove(true);
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
        isAttached = true;
        tileAstar.SetCanMove(true);
    }
}
