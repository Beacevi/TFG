/**
 * @file IsometricCamera.cs
 * @brief Controla el movimiento de la cámara isométrica y sus estados dentro de la isla procedural.
 * @author Hortensia Studio - Alejandro Romero Burgada
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using System.Collections;
using Unity.VisualScripting;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Controla el movimiento de la cámara isométrica y sus estados dentro de la isla procedural.
/// </summary>
public class IsometricCamera : MonoBehaviour
{
    /// <summary>
    /// Campo de tipo Transform utilizado para almacenar o configurar target.
    /// </summary>
    [SerializeField] Transform target;
    /// <summary>
    /// Campo de tipo Vector3 utilizado para almacenar o configurar offset.
    /// </summary>
    [SerializeField] Vector3 offset = new Vector3(25, 25, -25);
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar follow speed.
    /// </summary>
    [SerializeField] float followSpeed = 5f;

    /// <summary>
    /// Campo de tipo ChangeScene utilizado para almacenar o configurar change scene.
    /// </summary>
    [SerializeField] ChangeScene changeScene;
    /// <summary>
    /// Campo de tipo Vector3 utilizado para almacenar o configurar start pos.
    /// </summary>
    private Vector3 startPos;
    /// <summary>
    /// Indica si no more steps está activo o habilitado.
    /// </summary>
    bool noMoreSteps = false;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar distancia minima.
    /// </summary>
    float distanciaMinima = 0.1f;

    /// <summary>
    /// Campo de tipo TileAStar utilizado para almacenar o configurar tile astar.
    /// </summary>
    private TileAStar tileAstar;

    /// <summary>
    /// Campo de tipo Animator utilizado para almacenar o configurar animator.
    /// </summary>
    [SerializeField] private Animator animator;

    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar drag speed.
    /// </summary>
    [Header("Camera Control")]
    [SerializeField] float dragSpeed = 0.01f;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar zoom speed.
    /// </summary>
    [SerializeField] float zoomSpeed = 0.1f;
    /// <summary>
    /// Valor numérico que limita o define min zoom.
    /// </summary>
    [SerializeField] float minZoom = 5f;
    /// <summary>
    /// Valor numérico que limita o define max zoom.
    /// </summary>
    [SerializeField] float maxZoom = 20f;
    /// <summary>
    /// Valor numérico que limita o define max distance from target.
    /// </summary>
    [SerializeField] float maxDistanceFromTarget = 15f;

    /// <summary>
    /// Campo de tipo Camera utilizado para almacenar o configurar cam.
    /// </summary>
    private Camera cam;
    /// <summary>
    /// Indica si is attached está activo o habilitado.
    /// </summary>
    private bool isAttached = true;
    /// <summary>
    /// Campo de tipo Vector3 utilizado para almacenar o configurar last mouse pos.
    /// </summary>
    private Vector3 lastMousePos;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar initial zoom.
    /// </summary>
    private float initialZoom;

    /// <summary>
    /// Indica si is dragging está activo o habilitado.
    /// </summary>
    private bool isDragging = false;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar drag threshold.
    /// </summary>
    private float dragThreshold = 10f;

    /// <summary>
    /// Indica si input enabled está activo o habilitado.
    /// </summary>
    public static bool inputEnabled = true;

    /// <summary>
    /// Panel de interfaz asociado a panel pajaro conseguido.
    /// </summary>
    public GameObject panelPajaroConseguido;

    /// <summary>
    /// Panel de interfaz asociado a imagen panel pajaro conseguido.
    /// </summary>
    public Image imagenPanelPajaroConseguido;

    /// <summary>
    /// Campo de tipo TextMeshProUGUI utilizado para almacenar o configurar texto nombre pajaro.
    /// </summary>
    public TextMeshProUGUI textoNombrePajaro;

    /// <summary>
    /// Campo de tipo GameObject utilizado para almacenar o configurar boton salir menupajaro conseguido.
    /// </summary>
    public GameObject botonSalirMenupajaroConseguido;

    /// <summary>
    /// Valor numérico que limita o define minigame zoom.
    /// </summary>
    [Header("Minigame Zoom")]
    [SerializeField] float minigameZoom = 6f;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar zoom lerp speed.
    /// </summary>
    [SerializeField] float zoomLerpSpeed = 3f;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar target zoom.
    /// </summary>
    private float targetZoom;
    /// <summary>
    /// Indica si simon was loaded está activo o habilitado.
    /// </summary>
    private bool simonWasLoaded = false;

    /// <summary>
    /// Panel de interfaz asociado a panel resumen isla.
    /// </summary>
    [Header("Resumen Isla")]
    public GameObject panelResumenIsla;
    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar cantidad monedas.
    /// </summary>
    public int cantidadMonedas;
    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar cantidad pajaros.
    /// </summary>
    public int cantidadPajaros;
    /// <summary>
    /// Campo de tipo TextMeshProUGUI utilizado para almacenar o configurar texto birds.
    /// </summary>
    public TextMeshProUGUI textoBirds;
    /// <summary>
    /// Campo de tipo TextMeshProUGUI utilizado para almacenar o configurar texto coins.
    /// </summary>
    public TextMeshProUGUI textoCoins;

    /// <summary>
    /// Estados de cámara empleados para alternar entre modos de vista o interacción isométrica.
    /// </summary>
    enum CameraState
    {
        Following,
        Waiting,
        Returning,
        ChangingScene
    }

    /// <summary>
    /// Campo de tipo CameraState utilizado para almacenar o configurar state.
    /// </summary>
    CameraState state = CameraState.Following;

    /// <summary>
    /// Inicializa el componente cuando la escena ya está cargada y lista para comenzar.
    /// </summary>
    void Start()
    {
        cam = GetComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 10f;

        initialZoom = cam.orthographicSize;
        targetZoom = initialZoom;
        startPos = transform.position;
    }

    /// <summary>
    /// Ejecuta ajustes finales al terminar la actualización del fotograma.
    /// </summary>
    void LateUpdate()
    {
        if (tileAstar == null) return;

        HandleInput();

        // Zoom suave al entrar/salir del minijuego del pájaro
        bool simonLoaded = SimonGameManagerPajaro.IsActive;
        if (simonLoaded && !simonWasLoaded) targetZoom = minigameZoom;
        else if (!simonLoaded && simonWasLoaded) targetZoom = initialZoom;
        simonWasLoaded = simonLoaded;
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, zoomLerpSpeed * Time.deltaTime);

        // 🔹 Control de fin de movimiento (independiente de la cámara)
        if (tileAstar.stepsAvailable <= 0 && state == CameraState.Following)
        {
            target = null;
            state = CameraState.Waiting;
            StartCoroutine(WaitBeforeReturn());
        }

        switch (state)
        {
            case CameraState.Following:
                if (isAttached)
                    FollowPlayer();
                break;

            case CameraState.Returning:
                //ReturnToStart();
                panelResumenIsla.SetActive(true);
                textoBirds.text = string.Format("{0} + birds", cantidadPajaros);
                textoCoins.text = string.Format("{0} + coins", cantidadMonedas);
                break;
        }
    }
    /// <summary>
    /// Ejecuta la lógica asociada a handle input dentro de CameraState.
    /// </summary>
    void HandleInput()
    {
        if (SimonGameManagerPajaro.IsActive) return;
        if (IslandExitButton.IsConfirmOpen) return;

        if(tileAstar != null)
        {
            HandleMouse();
            HandleTouch();
        }
    }

    /// <summary>
    /// Ejecuta la lógica asociada a handle mouse dentro de CameraState.
    /// </summary>
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
                isAttached = true;
                tileAstar.SetCanMove(false);
            }
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            Zoom(scroll * 100f);
        }
    }

    /// <summary>
    /// Ejecuta la lógica asociada a handle touch dentro de CameraState.
    /// </summary>
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

            if (t.phase == TouchPhase.Ended)
            {
                isAttached = true;
                tileAstar.SetCanMove(true);
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

    /// <summary>
    /// Ejecuta la lógica asociada a move camera dentro de CameraState.
    /// </summary>
    /// <param name="delta">Parámetro delta empleado por el método.</param>
    void MoveCamera(Vector3 delta)
    {
        Vector3 move = new Vector3(-delta.x, -delta.y, 0) * dragSpeed;
        transform.Translate(move);

        ClampDistance();
    }

    /// <summary>
    /// Ejecuta la lógica asociada a clamp distance dentro de CameraState.
    /// </summary>
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

    /// <summary>
    /// Ejecuta la lógica asociada a zoom dentro de CameraState.
    /// </summary>
    /// <param name="increment">Parámetro increment empleado por el método.</param>
    void Zoom(float increment)
    {
        cam.orthographicSize -= increment;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
    }

    /// <summary>
    /// Restablece los valores del sistema a su configuración inicial o por defecto.
    /// </summary>
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

    /// <summary>
    /// Ejecuta la lógica asociada a follow player dentro de CameraState.
    /// </summary>
    void FollowPlayer()
    {
        if (target == null) return;

        Vector3 targetPosition = target.position + offset;transform.position = Vector3.Lerp(transform.position,targetPosition,followSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Ejecuta la lógica asociada a wait before return dentro de CameraState.
    /// </summary>
    /// <returns>Corrutina que permite ejecutar la operación de forma diferida en Unity.</returns>
    IEnumerator WaitBeforeReturn()
    {
        yield return new WaitForSeconds(1f);
        state = CameraState.Returning;
    }

    /// <summary>
    /// Ejecuta la lógica asociada a return to start dentro de CameraState.
    /// </summary>
    public void ReturnToStart()
    {
        Debug.Log("Cambiando de escena");
        state = CameraState.ChangingScene;
        changeScene.Cambiar_A_Escena("UI");
    }

    /// <summary>
    /// Ejecuta la lógica asociada a return to isla dentro de CameraState.
    /// </summary>
    public void ReturnToIsla()
    {
        panelPajaroConseguido.SetActive(false);
    }


    /// <summary>
    /// Ejecuta la lógica asociada a assing player dentro de CameraState.
    /// </summary>
    /// <param name="_player">Referencia al jugador afectado por la operación.</param>
    public void AssingPlayer(GameObject _player)
    {
        target = _player.transform;
        tileAstar = target.gameObject.GetComponent<TileAStar>();
        isAttached = true;
        tileAstar.SetCanMove(true);
        transform.position = target.position + offset;
    }

    /// <summary>
    /// Ejecuta la lógica asociada a mostrar panel pajaro conseguido dentro de CameraState.
    /// </summary>
    /// <param name="imagenPajaroConseguido">Parámetro imagen pajaro conseguido empleado por el método.</param>
    /// <param name="nombrePajaro">Parámetro nombre pajaro empleado por el método.</param>
    /// <returns>Corrutina que permite ejecutar la operación de forma diferida en Unity.</returns>
    public IEnumerator MostrarPanelPajaroConseguido(SpriteRenderer imagenPajaroConseguido, string nombrePajaro)
    {
        imagenPanelPajaroConseguido.sprite = imagenPajaroConseguido.sprite;

        textoNombrePajaro.text = nombrePajaro;

        panelPajaroConseguido.SetActive(true);

        yield return new WaitForSeconds(5f);
        //botonSalirMenupajaroConseguido.SetActive(true);

        panelPajaroConseguido.SetActive(false);
    }

}
