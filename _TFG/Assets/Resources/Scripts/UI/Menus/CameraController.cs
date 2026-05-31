/**
 * @file CameraController.cs
 * @brief Controla el desplazamiento o enfoque de la cámara en escenas de menú.
 * @author Hortensia Studio
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using UnityEngine;

/// <summary>
/// Controla el desplazamiento o enfoque de la cámara en escenas de menú.
/// </summary>
public class CameraController : MonoBehaviour
{
    /// <summary>
    /// Campo de tipo Vector2 utilizado para almacenar o configurar bounds center.
    /// </summary>
    [Header("Bounds")]
    public Vector2 boundsCenter = Vector2.zero;
    /// <summary>
    /// Campo de tipo Vector2 utilizado para almacenar o configurar bounds size.
    /// </summary>
    public Vector2 boundsSize = new Vector2(20f, 12f);

    /// <summary>
    /// Valor numérico que limita o define min zoom.
    /// </summary>
    [Header("Zoom")]
    public float minZoom = 3f;
    /// <summary>
    /// Valor numérico que limita o define max zoom.
    /// </summary>
    public float maxZoom = 10f;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar mouse scroll sensitivity.
    /// </summary>
    public float mouseScrollSensitivity = 1f;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar pinch sensitivity.
    /// </summary>
    public float pinchSensitivity = 0.05f;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar zoom smooth speed.
    /// </summary>
    public float zoomSmoothSpeed = 8f;

    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar inertia damping.
    /// </summary>
    [Header("Inertia")]
    public float inertiaDamping = 0.92f;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar inertia min speed.
    /// </summary>
    public float inertiaMinSpeed = 0.01f;

    /// <summary>
    /// Campo de tipo Camera utilizado para almacenar o configurar cam.
    /// </summary>
    private Camera cam;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar target zoom.
    /// </summary>
    private float targetZoom;
    /// <summary>
    /// Campo de tipo Vector3 utilizado para almacenar o configurar inertia velocity.
    /// </summary>
    private Vector3 inertiaVelocity;
    /// <summary>
    /// Campo de tipo Vector2 utilizado para almacenar o configurar last mouse pos.
    /// </summary>
    private Vector2 lastMousePos;
    /// <summary>
    /// Indica si is panning está activo o habilitado.
    /// </summary>
    private bool isPanning;

    /// <summary>
    /// Inicializa el componente cuando la escena ya está cargada y lista para comenzar.
    /// </summary>
    private void Start()
    {
        cam = GetComponent<Camera>();
        maxZoom = Mathf.Min(maxZoom, GetBoundsMaxZoom());
        targetZoom = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
    }

    /// <summary>
    /// Actualiza la lógica del componente en cada fotograma.
    /// </summary>
    private void Update()
    {
        HandleZoom();
        HandlePan();
        ApplyInertia();
        ClampPosition();
    }

    // ─── Converts a screen-space pixel delta to world-space units ─────────
    // Avoids ScreenToWorldPoint entirely — no Z issues possible

    /// <summary>
    /// Ejecuta la lógica asociada a screen delta to world dentro de CameraController.
    /// </summary>
    /// <param name="screenDelta">Parámetro screen delta empleado por el método.</param>
    /// <returns>Instancia o valor de tipo Vector2 resultante de la operación.</returns>
    private Vector2 ScreenDeltaToWorld(Vector2 screenDelta)
    {
        float worldUnitsPerPixel = (cam.orthographicSize * 2f) / Screen.height;
        return screenDelta * worldUnitsPerPixel;
    }

    // ─── Zoom ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Ejecuta la lógica asociada a handle zoom dentro de CameraController.
    /// </summary>
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

    /// <summary>
    /// Ejecuta la lógica asociada a handle pan dentro de CameraController.
    /// </summary>
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

    /// <summary>
    /// Ejecuta la lógica asociada a apply inertia dentro de CameraController.
    /// </summary>
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

    /// <summary>
    /// Ejecuta la lógica asociada a clamp position dentro de CameraController.
    /// </summary>
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

    /// <summary>
    /// Obtiene bounds max zoom a partir del estado actual del sistema.
    /// </summary>
    /// <returns>Valor numérico calculado o consultado por el método.</returns>
    float GetBoundsMaxZoom()
    {
        float maxByHeight = boundsSize.y / 2f;
        float maxByWidth = boundsSize.x / 2f / cam.aspect;
        return Mathf.Min(maxByHeight, maxByWidth);
    }

#if UNITY_EDITOR
    /// <summary>
    /// Ejecuta la lógica asociada a on draw gizmos selected dentro de CameraController.
    /// </summary>
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
