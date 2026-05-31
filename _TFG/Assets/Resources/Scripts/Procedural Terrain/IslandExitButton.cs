/**
 * @file IslandExitButton.cs
 * @brief Gestiona el botón de salida de la isla y el retorno a la interfaz principal.
 * @author Hortensia Studio
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using UnityEngine;

/// <summary>
/// Botón para abandonar la isla (escena del mapa procedural). Muestra un panel
/// de confirmación antes de cambiar de escena para evitar salidas accidentales.
/// Mientras el panel está abierto, expone <see cref="IsConfirmOpen"/> = true para
/// que <see cref="TileAStar"/> e <see cref="IsometricCamera"/> ignoren clics y arrastres.
/// </summary>
public class IslandExitButton : MonoBehaviour
{
    /// <summary>
    /// Panel de interfaz asociado a exit confirm panel.
    /// </summary>
    [Tooltip("Panel con los botones Sí/No de confirmación.")]
    [SerializeField] private GameObject exitConfirmPanel;

    /// <summary>
    /// Botón de interfaz asociado a main exit button.
    /// </summary>
    [Tooltip("Botón principal de salir de la isla. Se oculta automáticamente mientras el jugador está dentro del minijuego de pájaros.")]
    [SerializeField] private GameObject mainExitButton;

    /// <summary>
    /// Campo de tipo ChangeScene utilizado para almacenar o configurar change scene.
    /// </summary>
    [Tooltip("Referencia al ChangeScene. Si se deja vacío, se busca por tag SceneChanger.")]
    [SerializeField] private ChangeScene changeScene;

    /// <summary>
    /// Campo de tipo string utilizado para almacenar o configurar target scene.
    /// </summary>
    [Tooltip("Nombre de la escena a la que volver al confirmar la salida.")]
    [SerializeField] private string targetScene = "UI";

    /// <summary>Cierto mientras el panel de confirmación está abierto.</summary>
    public static bool IsConfirmOpen { get; private set; }

    /// <summary>
    /// Inicializa referencias internas antes de que comience la ejecución normal del componente.
    /// </summary>
    private void Awake()
    {
        // Reset al cargar la escena por si el flag quedó activo de una sesión anterior.
        IsConfirmOpen = false;
    }

    /// <summary>
    /// Inicializa el componente cuando la escena ya está cargada y lista para comenzar.
    /// </summary>
    private void Start()
    {
        if (changeScene == null)
        {
            var go = GameObject.FindGameObjectWithTag("SceneChanger");
            if (go != null) changeScene = go.GetComponent<ChangeScene>();
        }

        if (exitConfirmPanel != null) exitConfirmPanel.SetActive(false);

        // Fallback: si el campo no se asignó desde el Inspector / el menú de Editor,
        // intentar localizar el botón principal por nombre en el primer Canvas que aparezca.
        if (mainExitButton == null)
        {
            var canvas = FindFirstObjectByType<Canvas>();
            if (canvas != null)
            {
                var found = canvas.transform.Find("BotonSalirIsla");
                if (found != null) mainExitButton = found.gameObject;
            }
        }
    }

    /// <summary>
    /// Actualiza la lógica del componente en cada fotograma.
    /// </summary>
    private void Update()
    {
        // Ocultar el botón principal de salir mientras el minijuego de pájaros está activo
        // y volverlo a mostrar cuando termine (por éxito, fallo o cancelación voluntaria).
        if (mainExitButton == null) return;

        bool shouldShow = !SimonGameManagerPajaro.IsActive;
        if (mainExitButton.activeSelf != shouldShow)
        {
            mainExitButton.SetActive(shouldShow);
        }
    }

    /// <summary>Botón principal de "Salir de la isla". Muestra el panel.</summary>
    public void OnExitButtonPressed()
    {
        if (exitConfirmPanel != null) exitConfirmPanel.SetActive(true);
        IsConfirmOpen = true;
    }

    /// <summary>Botón "Sí" del panel: cambia a la escena destino.</summary>
    public void ConfirmExit()
    {
        IsConfirmOpen = false;
        if (exitConfirmPanel != null) exitConfirmPanel.SetActive(false);

        // Evitar que el clic sobre "Sí" se propague al tilemap durante el cambio de
        // escena y mueva al jugador antes de que termine la animación de las nubes.
        TileAStar.BlockInputForSeconds(0.3f);

        if (changeScene != null)
        {
            changeScene.Cambiar_A_Escena(targetScene);
        }
        else
        {
            Debug.LogWarning("[IslandExitButton] No hay ChangeScene asignado ni se encontró por tag.");
        }
    }

    /// <summary>Botón "No" del panel: vuelve al mapa sin cambiar de escena.</summary>
    public void CancelExit()
    {
        IsConfirmOpen = false;
        if (exitConfirmPanel != null) exitConfirmPanel.SetActive(false);

        // Mismo motivo que en ConfirmExit: cuando se cierra el panel, el clic ya
        // no encuentra UI encima del puntero y movería al jugador a la casilla
        // bajo el botón si no bloqueamos el input un instante.
        TileAStar.BlockInputForSeconds(0.3f);
    }
}
