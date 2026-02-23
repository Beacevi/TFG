using UnityEngine;

public class UIFollowWorldObject : MonoBehaviour
{
    public Transform target;
    public RectTransform uiToMove;   // BlackboardRoot
    public Canvas canvas;

    Camera cam;
    RectTransform canvasRect;

    void Awake()
    {
        cam = Camera.main;
        canvasRect = canvas.GetComponent<RectTransform>();
    }

    void LateUpdate()
    {
        if (target == null)
            return;

        // World → Screen
        Vector3 screenPos = cam.WorldToScreenPoint(target.position);

        // Hide if behind camera
        if (screenPos.z < 0)
        {
            uiToMove.gameObject.SetActive(false);
            return;
        }

        uiToMove.gameObject.SetActive(true);

        // Screen → Canvas local
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : cam,
            out localPoint
        );

        uiToMove.anchoredPosition = localPoint;
    }
}