using UnityEngine;

public class CanvasSwitcher : MonoBehaviour
{
    [Header("Canvases")]
    public Canvas desktopCanvas;
    public Canvas mobileCanvas;

    [Header("Settings")]
    [Tooltip("Aspect ratio below this value is considered portrait/mobile")]
    public float portraitThreshold = 1f;

    private bool lastMobileState;

    private void Awake()
    {
        Apply();
    }

    private void Update()
    {
        // Re-evaluate every frame to handle
        // window resizing on PC and rotation on mobile
        Apply();
    }

    private void Apply()
    {
        bool isMobile = IsMobileLayout();

        if (isMobile == lastMobileState) return;

        lastMobileState = isMobile;
        desktopCanvas.gameObject.SetActive(!isMobile);
        mobileCanvas.gameObject.SetActive(isMobile);
    }

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