/**
 * @file CanvasSwitcher.cs
 * @brief Activa y desactiva distintos canvas de interfaz según la navegación del usuario.
 * @author Hortensia Studio
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using UnityEngine;

/// <summary>
/// Activa y desactiva distintos canvas de interfaz según la navegación del usuario.
/// </summary>
public class CanvasSwitcher : MonoBehaviour
{
    /// <summary>
    /// Campo de tipo Canvas utilizado para almacenar o configurar desktop canvas.
    /// </summary>
    [Header("Canvases")]
    public Canvas desktopCanvas;
    /// <summary>
    /// Campo de tipo Canvas utilizado para almacenar o configurar mobile canvas.
    /// </summary>
    public Canvas mobileCanvas;

    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar portrait threshold.
    /// </summary>
    [Header("Settings")]
    [Tooltip("Aspect ratio below this value is considered portrait/mobile")]
    public float portraitThreshold = 1f;

    /// <summary>
    /// Indica si last mobile state está activo o habilitado.
    /// </summary>
    private bool lastMobileState;

    /// <summary>
    /// Inicializa referencias internas antes de que comience la ejecución normal del componente.
    /// </summary>
    private void Awake()
    {
        Apply();
    }

    /// <summary>
    /// Actualiza la lógica del componente en cada fotograma.
    /// </summary>
    private void Update()
    {
        // Re-evaluate every frame to handle
        // window resizing on PC and rotation on mobile
        Apply();
    }

    /// <summary>
    /// Ejecuta la lógica asociada a apply dentro de CanvasSwitcher.
    /// </summary>
    private void Apply()
    {
        bool isMobile = IsMobileLayout();

        if (isMobile == lastMobileState) return;

        lastMobileState = isMobile;
        desktopCanvas.gameObject.SetActive(!isMobile);
        mobileCanvas.gameObject.SetActive(isMobile);
    }

    /// <summary>
    /// Ejecuta la lógica asociada a is mobile layout dentro de CanvasSwitcher.
    /// </summary>
    /// <returns>true si la operación se ha completado correctamente; false en caso contrario.</returns>
    private bool IsMobileLayout()
    {
        // On an actual mobile device, also check orientation
        // On PC, check if the window is portrait-shaped
        float aspectRatio = (float)Screen.width / Screen.height;
        bool isPortrait = aspectRatio < portraitThreshold;
        bool isMobileDevice = SystemInfo.deviceType == DeviceType.Handheld;

        return isMobileDevice || isPortrait;
    }
}
