using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Bounds")]
    public Vector2 boundsCenter = Vector2.zero;
    public Vector2 boundsSize = new Vector2(20f, 12f);

    [Header("Zoom")]
    public float minZoom = 3f;
    public float maxZoom = 10f;
    public float mouseScrollSensitivity = 1f;
    public float pinchSensitivity = 0.05f;
    public float zoomSmoothSpeed = 8f;

    [Header("Inertia")]
    public float inertiaDamping = 0.92f;
    public float inertiaMinSpeed = 0.01f;

    private Camera cam;
    private float targetZoom;
    private Vector3 inertiaVelocity;
    private Vector2 lastMousePos;
    private bool isPanning;

    private void Start()
    {
        cam = GetComponent<Camera>();
        maxZoom = Mathf.Min(maxZoom, GetBoundsMaxZoom());
        targetZoom = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
    }

    private void Update()
    {
        HandleZoom();
        HandlePan();
        ApplyInertia();
        ClampPosition();
    }

    // ─── Converts a screen-space pixel delta to world-space units ─────────
    // Avoids ScreenToWorldPoint entirely — no Z issues possible

    private Vector2 ScreenDeltaToWorld(Vector2 screenDelta)
    {
        float worldUnitsPerPixel = (cam.orthographicSize * 2f) / Screen.height;
        return screenDelta * worldUnitsPerPixel;
    }

    // ─── Zoom ─────────────────────────────────────────────────────────────

    void HandleZoom()
    {
        // PC — scroll wheel
        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > 0f)
        {
            targetZoom -= scroll * mouseScrollSensitivity;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
            inertiaVelocity = Vector3.zero;
            isPanning = false;
        }

        // Mobile — pinch
        if (Input.touchCount == 2)
        {
            inertiaVelocity = Vector3.zero;

            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            float currentDist = Vector2.Distance(t0.position, t1.position);
            float previousDist = Vector2.Distance(
                t0.position - t0.deltaPosition,
                t1.position - t1.deltaPosition
            );

            targetZoom += (previousDist - currentDist) * pinchSensitivity;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        }

        cam.orthographicSize = Mathf.Lerp(
            cam.orthographicSize, targetZoom, Time.deltaTime * zoomSmoothSpeed
        );
    }

    // ─── Pan ──────────────────────────────────────────────────────────────

    void HandlePan()
    {
        if (Input.touchCount == 2) return;

        // PC — mouse drag
        if (Input.GetMouseButtonDown(0))
        {
            lastMousePos = Input.mousePosition;
            isPanning = true;
            inertiaVelocity = Vector3.zero;
        }

        if (Input.GetMouseButton(0) && isPanning)
        {
            Vector2 screenDelta = (Vector2)Input.mousePosition - lastMousePos;
            Vector2 worldDelta = ScreenDeltaToWorld(screenDelta);

            // Negative because dragging right should move the world right
            // (camera moves left)
            transform.position -= new Vector3(worldDelta.x, worldDelta.y, 0f);
            inertiaVelocity = new Vector3(-worldDelta.x, -worldDelta.y, 0f)
                                  / Time.deltaTime * 0.1f;

            lastMousePos = Input.mousePosition;
            ClampPosition();
        }

        if (Input.GetMouseButtonUp(0))
            isPanning = false;

        // Mobile — single finger drag
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
                inertiaVelocity = Vector3.zero;

            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 worldDelta = ScreenDeltaToWorld(touch.deltaPosition);

                transform.position -= new Vector3(worldDelta.x, worldDelta.y, 0f);
                inertiaVelocity = new Vector3(-worldDelta.x, -worldDelta.y, 0f)
                                      / Time.deltaTime * 0.1f;

                ClampPosition();
            }
        }
    }

    // ─── Inertia ──────────────────────────────────────────────────────────

    void ApplyInertia()
    {
        if (Input.GetMouseButton(0) || Input.touchCount > 0) return;

        if (inertiaVelocity.magnitude < inertiaMinSpeed)
        {
            inertiaVelocity = Vector3.zero;
            return;
        }

        transform.position += inertiaVelocity * Time.deltaTime;
        inertiaVelocity *= inertiaDamping;
        ClampPosition();
    }

    // ─── Bounds ───────────────────────────────────────────────────────────

    void ClampPosition()
    {
        float halfH = cam.orthographicSize;
        float halfW = cam.orthographicSize * cam.aspect;

        float minX = boundsCenter.x - boundsSize.x / 2f + halfW;
        float maxX = boundsCenter.x + boundsSize.x / 2f - halfW;
        float minY = boundsCenter.y - boundsSize.y / 2f + halfH;
        float maxY = boundsCenter.y + boundsSize.y / 2f - halfH;

        float clampedX = Mathf.Clamp(transform.position.x, minX, maxX);
        float clampedY = Mathf.Clamp(transform.position.y, minY, maxY);

        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }

    float GetBoundsMaxZoom()
    {
        float maxByHeight = boundsSize.y / 2f;
        float maxByWidth = boundsSize.x / 2f / cam.aspect;
        return Mathf.Min(maxByHeight, maxByWidth);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(
            new Vector3(boundsCenter.x, boundsCenter.y, 0f),
            new Vector3(boundsSize.x, boundsSize.y, 0f)
        );
    }
#endif
}